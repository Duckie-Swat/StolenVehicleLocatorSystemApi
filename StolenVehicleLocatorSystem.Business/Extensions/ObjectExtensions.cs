using System.Reflection.Emit;
using System.Reflection;

namespace StolenVehicleLocatorSystem.Business.Extensions
{
    /// <summary>
    /// This extension using similar to spread operator in javascript 
    ///  var cliente = new Brazilian { Nome = "Bob", Id = Guid.Parse("c301f874-007f-4b40-a191-56216e6ba98c") };
    ///  var cliente2 = new American { Nome = "Leandro", Id = "A7D8" };
    ///  var endereco = new Endereco { Logradouro = "Rua Mole", Numero = 123 };
    ///  var opa = cliente.Spread(cliente2, endereco, new { UsaGitHub = true });
    ///  Console.WriteLine(JsonSerializer.Serialize(opa));
    ///  Result: { "Nome":"Leandro","Logradouro":"Rua Mole","Numero":123,"UsaGitHub":true}
    /// references: https://gist.github.com/leandromoh/d932eff216fc7a3ebf2133a02fa1efd5
    /// </summary>
    public static class ObjectExtensions
    {
        static ObjectExtensions()
        {
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("AssemblyX"), AssemblyBuilderAccess.RunAndCollect);
            ModuleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
        }

        public static dynamic Spread(this object obj, params object[] anotherObject)
        {
            var allobjs = anotherObject.Prepend(obj);

            var _base = allobjs.SelectMany(x => x.GetType().GetInterfaces()).ToArray();

            var interfaces = allobjs.SelectMany(o => o.GetType().GetInterfaces(), (o, i) => (o, i));
            var propsInter = interfaces.SelectMany(x => x.i.GetProperties(), (x, p) => (x.o, p)).Reverse().DistinctBy(x => x.p).ToArray();

            var propertiesNotInterface = allobjs
                .SelectMany(o => o.GetType().GetProperties(), (o, p) => (o: o, p: p))
                .Where(x =>
                {
                    var result = _base.Any(i => i.IsAssignableFrom(x.p.DeclaringType) && i.GetProperty(x.p.Name) != null);

                    return result is false;
                })
                .ToArray();


            var all = propsInter
                .Concat(propertiesNotInterface).Select(x => x.p);

            var objType = CreateClass(_base, all);

            var finalObj = Activator.CreateInstance(objType);

            var none = propertiesNotInterface
                .ToDictionary(t => (t.p.DeclaringType, t.p), t => t.p.GetValue(t.o));

            var fromInte = propsInter
                .ToDictionary(t => (t.p.DeclaringType, t.p.GetSetMethod()), t => t.p.GetValue(t.o));

            foreach (var prop in none)
            {
                objType.GetProperty(prop.Key.p.Name).SetValue(finalObj, prop.Value);
            }

            foreach (var inter in _base)
            {
                InterfaceMapping map = objType.GetInterfaceMap(inter);

                for (int i = 0; i < map.InterfaceMethods.Length; i++)
                {
                    MethodInfo ifaceMethod = map.InterfaceMethods[i];
                    MethodInfo targetMethod = map.TargetMethods[i];

                    if (fromInte.TryGetValue((inter, ifaceMethod), out var value))
                    {
                        targetMethod.Invoke(finalObj, new[] { value });
                    }
                }
            }

            return finalObj;
        }

        const MethodAttributes METHOD_ATTRIBUTES = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
        private static ModuleBuilder ModuleBuilder;

        internal static Type CreateClass(Type[] _base, IEnumerable<PropertyInfo> parameters)
        {
            var typeBuilder = ModuleBuilder.DefineType(Guid.NewGuid().ToString(), TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout, null, interfaces: _base);
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            foreach (var parameter in parameters)
                CreateProperty(typeBuilder, parameter);

            var type = typeBuilder.CreateTypeInfo().AsType();
            return type;
        }

        private static PropertyBuilder CreateProperty(TypeBuilder typeBuilder, PropertyInfo prop)
        {
            var fieldBuilder = typeBuilder.DefineField($"<{prop.DeclaringType.Name}>" + prop.Name, prop.PropertyType, FieldAttributes.Private);

            var propBuilder = typeBuilder.DefineProperty(prop.Name, PropertyAttributes.HasDefault, prop.PropertyType, null);

            propBuilder.SetGetMethod(DefineGet(typeBuilder, fieldBuilder, propBuilder, prop.GetGetMethod()));
            propBuilder.SetSetMethod(DefineSet(typeBuilder, fieldBuilder, propBuilder, prop.GetSetMethod()));

            return propBuilder;
        }

        private static MethodBuilder DefineSet(TypeBuilder typeBuilder, FieldBuilder fieldBuilder, PropertyBuilder propBuilder, MethodInfo m)
            => DefineMethod(typeBuilder, $"set_{propBuilder.Name}", null, new[] { propBuilder.PropertyType }, m, il =>
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, fieldBuilder);
                il.Emit(OpCodes.Ret);
            });

        private static MethodBuilder DefineGet(TypeBuilder typeBuilder, FieldBuilder fieldBuilder, PropertyBuilder propBuilder, MethodInfo m)
            => DefineMethod(typeBuilder, $"get_{propBuilder.Name}", propBuilder.PropertyType, Type.EmptyTypes, m, il =>
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fieldBuilder);
                il.Emit(OpCodes.Ret);
            });

        private static MethodBuilder DefineMethod(TypeBuilder typeBuilder, string methodName, Type propertyType, Type[] parameterTypes, MethodInfo m, Action<ILGenerator> bodyWriter)
        {
            var m2 = m?.ReflectedType?.IsInterface is true
                    ? m.ReflectedType.GetMethod(m.Name)
                    : null;

            var attr = m2 is null
                        ? METHOD_ATTRIBUTES
                        : MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual;

            var methodBuilder = typeBuilder.DefineMethod(methodName, attr, propertyType, parameterTypes);

            bodyWriter(methodBuilder.GetILGenerator());

            if (m2 != null)
            {
                typeBuilder.DefineMethodOverride(methodBuilder, m2);
            }

            return methodBuilder;
        }
    }
}
