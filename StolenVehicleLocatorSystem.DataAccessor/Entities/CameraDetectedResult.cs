﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace StolenVehicleLocatorSystem.DataAccessor.Entities
{
    public class CameraDetectedResult : BaseEntity
    {
        public Guid CameraId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Location { get; set; }

        public string Photo { get; set; }

        [ForeignKey("CameraId")]
        public Camera Camera { get; set; }
    }
}