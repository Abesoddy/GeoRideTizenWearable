using System;

namespace GeoRideTizenWearable.Models
{
    public class TrackerModel
    {
        public int TrackerId { get; set; }
        public string TrackerName { get; set; }
        public string DeviceButtonAction { get; set; }
        public int? DeviceButtonDelay { get; set; }
        public int? VibrationLevel { get; set; }
        public bool IsOldTracker { get; set; }
        public DateTime? Fixtime { get; set; }
        public string Role { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public int? GiftCardId { get; set; }
        public DateTime? Expires { get; set; }
        public DateTime? ActivationDate { get; set; }
        public int? Odometer { get; set; }
        public bool IsStolen { get; set; }
        public bool IsCrashed { get; set; }
        public bool CrashDetectionDisabled { get; set; }
        public int? Speed { get; set; }
        public bool Moving { get; set; }
        public int? PositionId { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public int? Altitude { get; set; }
        public int? LockedPositionId { get; set; }
        public float? LockedLatitude { get; set; }
        public float? LockedLongitude { get; set; }
        public bool IsLocked { get; set; }
        public bool CanSeePosition { get; set; }
        public bool CanLock { get; set; }
        public bool CanUnlock { get; set; }
        public bool CanShare { get; set; }
        public bool CanUnshare { get; set; }
        public bool CanCheckSpeed { get; set; }
        public bool CanSeeStatistics { get; set; }
        public bool CanSendBrokenDownSignal { get; set; }
        public bool CanSendStolenSignal { get; set; }
        public string Status { get; set; }
    }
}