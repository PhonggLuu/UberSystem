﻿namespace UberSystem.Dto.Requests
{
	public class LocationRequest
	{
		public string PickUpAddress { get; set; } = string.Empty;
		public string PickUpWard { get; set; } = string.Empty;
		public string DropOffAddress { get; set; } = string.Empty;
		public string DropOffWard { get; set; } = string.Empty;

	}
}
