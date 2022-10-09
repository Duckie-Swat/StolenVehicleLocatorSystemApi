select cdr."Id", cdr."CameraId", cdr."Latitude", cdr."Longitude", cdr."Location", cdr."Photo", cdr."CreatedAt", cdr."LastUpdatedAt", cdr."CreatedBy", cdr."LastUpdatedBy", cdr."DeletedBy", cdr."IsDeleted", cdr."LostVehicleRequestId", cdr."PlateNumber" 
from "CameraDetectedResult" cdr , "LostVehicleRequests" lvr 
where cdr."LostVehicleRequestId" = lvr."Id" and lvr."UserId" = uuid('a5b494fd-5f23-409c-b3fa-313acf45889e') 
and cdr."CreatedAt" in 
(select max(cdr2."CreatedAt") from "CameraDetectedResult" cdr2, "LostVehicleRequests" lvr2 
where  cdr2."LostVehicleRequestId" = lvr2."Id" and lvr2."UserId" = uuid('a5b494fd-5f23-409c-b3fa-313acf45889e')
group by cdr2."PlateNumber"); 