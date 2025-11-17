using System.Reflection;

namespace _1iveowl.Models;

public class AvtransportScpd
{
    private static readonly StateVariable A_ARG_TYPE_InstanceID = new() {name = "A_ARG_TYPE_InstanceID", dataType = "ui4"};
    private static readonly Argument InstanceID = new() { name = "InstanceID", direction = "in", relatedStateVariable = A_ARG_TYPE_InstanceID.name };

    private static readonly StateVariable AVTransportURI = new() {name = "AVTransportURI", dataType = "string"};
    private static readonly Argument CurrentURIIn = new() { name = "CurrentURI", direction = "in", relatedStateVariable = AVTransportURI.name };
    private static readonly Argument CurrentURIOut = new() { name = "CurrentURI", direction = "out", relatedStateVariable = AVTransportURI.name };

    private static readonly StateVariable AVTransportURIMetaData = new() {name = "AVTransportURIMetaData", dataType = "string", defaultValue = "NOT_IMPLEMENTED"};
    private static readonly Argument CurrentURIMetaDataIn = new() { name = "CurrentURIMetaData", direction = "in", relatedStateVariable = AVTransportURIMetaData.name };
    private static readonly Argument CurrentURIMetaDataOut = new() { name = "CurrentURIMetaData", direction = "out", relatedStateVariable = AVTransportURIMetaData.name };

    private static readonly StateVariable TransportPlaySpeed = new() {name = "TransportPlaySpeed", dataType = "string", defaultValue = "1"};
    private static readonly Argument Speed = new() { name = "Speed", direction = "in", relatedStateVariable = TransportPlaySpeed.name };
    private static readonly Argument CurrentSpeed = new() { name = "CurrentSpeed", direction = "out", relatedStateVariable = TransportPlaySpeed.name };

    private static readonly StateVariable TransportState = new() {name = "TransportState", dataType = "string", allowedValues = [
        "STOPPED", "PLAYING", "TRANSITIONING", "PAUSED_PLAYBACK", "PAUSED_RECORDING", "RECORDING", "NO_MEDIA_PRESENT", "CUSTOM"]};
    private static readonly Argument CurrentTransportState = new() { name = "CurrentTransportState", direction = "out", relatedStateVariable = TransportState.name };

    private static readonly StateVariable TransportStatus = new() {name = "TransportStatus", dataType = "string", allowedValues = [
        "OK", "ERROR_OCCURRED", "CUSTOM"]};
    private static readonly Argument CurrentTransportStatus = new() { name = "CurrentTransportStatus", direction = "out", relatedStateVariable = TransportStatus.name };

    private static readonly StateVariable CurrentTrack = new() {name = "CurrentTrack", dataType = "ui4", defaultValue = "0"};
    private static readonly Argument Track = new() { name = "Track", direction = "out", relatedStateVariable = CurrentTrack.name };

    private static readonly StateVariable CurrentTrackDuration = new() {name = "CurrentTrackDuration", dataType = "string"};
    private static readonly Argument TrackDuration = new() { name = "TrackDuration", direction = "out", relatedStateVariable = CurrentTrackDuration.name };

    private static readonly StateVariable CurrentTrackMetaData = new() {name = "CurrentTrackMetaData", dataType = "string", defaultValue = "NOT_IMPLEMENTED"};
    private static readonly Argument TrackMetaData = new() { name = "TrackMetaData", direction = "out", relatedStateVariable = CurrentTrackMetaData.name };

    private static readonly StateVariable CurrentTrackURI = new() {name = "CurrentTrackURI", dataType = "string"};
    private static readonly Argument TrackURI = new() { name = "TrackURI", direction = "out", relatedStateVariable = CurrentTrackURI.name };

    private static readonly StateVariable RelativeTimePosition = new() {name = "RelativeTimePosition", dataType = "string"};
    private static readonly Argument RelTime = new() { name = "RelTime", direction = "out", relatedStateVariable = RelativeTimePosition.name };

    private static readonly StateVariable AbsoluteTimePosition = new() {name = "AbsoluteTimePosition", dataType = "string"};
    private static readonly Argument AbsTime = new() { name = "AbsTime", direction = "out", relatedStateVariable = AbsoluteTimePosition.name };

    private static readonly StateVariable RelativeCounterPosition = new() {name = "RelativeCounterPosition", dataType = "i4", defaultValue = "2147483647"};
    private static readonly Argument RelCount = new() { name = "RelCount", direction = "out", relatedStateVariable = RelativeCounterPosition.name };

    private static readonly StateVariable AbsoluteCounterPosition = new() {name = "AbsoluteCounterPosition", dataType = "i4", defaultValue = "2147483647"};
    private static readonly Argument AbsCount = new() { name = "AbsCount", direction = "out", relatedStateVariable = AbsoluteCounterPosition.name };

    private static readonly StateVariable NumberOfTracks = new() {name = "NumberOfTracks", dataType = "ui4", defaultValue = "0"};
    private static readonly Argument NrTracks = new() { name = "NrTracks", direction = "out", relatedStateVariable = NumberOfTracks.name };

    private static readonly StateVariable CurrentMediaDuration = new() {name = "CurrentMediaDuration", dataType = "string", defaultValue = "00:00:00"};
    private static readonly Argument MediaDuration = new() { name = "MediaDuration", direction = "out", relatedStateVariable = CurrentMediaDuration.name };

    private static readonly StateVariable NextAVTransportURI = new() {name = "NextAVTransportURI", dataType = "string", defaultValue = "NOT_IMPLEMENTED"};
    private static readonly Argument NextURI = new() { name = "NextURI", direction = "out", relatedStateVariable = NextAVTransportURI.name };

    private static readonly StateVariable NextAVTransportURIMetaData = new() {name = "NextAVTransportURIMetaData", dataType = "string", defaultValue = "NOT_IMPLEMENTED"};
    private static readonly Argument NextURIMetaData = new() { name = "NextURIMetaData", direction = "out", relatedStateVariable = NextAVTransportURIMetaData.name };

    private static readonly StateVariable PlaybackStorageMedium = new() {name = "PlaybackStorageMedium", dataType = "string", defaultValue = "NONE", allowedValues = [
        "UNKNOWN", "NETWORK", "NONE", "NOT_IMPLEMENTED", "VENDOR_SPECIFIC"]};
    private static readonly Argument PlayMedium = new() { name = "PlayMedium", direction = "out", relatedStateVariable = PlaybackStorageMedium.name };

    private static readonly StateVariable RecordStorageMedium = new() {name = "RecordStorageMedium", dataType = "string", defaultValue = "NOT_IMPLEMENTED", allowedValues = [
        "UNKNOWN", "NETWORK", "NONE", "NOT_IMPLEMENTED", "VENDOR_SPECIFIC"]};
    private static readonly Argument RecordMedium = new() { name = "RecordMedium", direction = "out", relatedStateVariable = RecordStorageMedium.name };

    private static readonly StateVariable RecordMediumWriteStatus = new() {name = "RecordMediumWriteStatus", dataType = "string", defaultValue = "NOT_IMPLEMENTED", allowedValues = [
        "WRITABLE", "PROTECTED", "NOT_WRITABLE", "UNKNOWN", "NOT_IMPLEMENTED"]};
    private static readonly Argument WriteStatus = new() { name = "WriteStatus", direction = "out", relatedStateVariable = RecordMediumWriteStatus.name };

    private static readonly StateVariable LastChange = new() {name = "LastChange", dataType = "string", sendEvents = "yes"};

    public static readonly Scpd SCPD = new() {
        actions = [
            new ScpdAction{name = "SetAVTransportURI", arguments = [InstanceID, CurrentURIIn, CurrentURIMetaDataIn]},
            new ScpdAction{name = "Play", arguments = [InstanceID, Speed]},
            new ScpdAction{name = "Pause", arguments = [InstanceID]},
            new ScpdAction{name = "Stop", arguments = [InstanceID]},
            new ScpdAction{name = "Next", arguments = [InstanceID]},
            new ScpdAction{name = "Previous", arguments = [InstanceID]},
            new ScpdAction{name = "GetTransportInfo", arguments = [InstanceID, CurrentTransportState, CurrentTransportStatus, CurrentSpeed]},
            new ScpdAction{name = "GetPositionInfo", arguments = [InstanceID, Track, TrackDuration, TrackMetaData, TrackURI, RelTime, AbsTime, RelCount, AbsCount]},
            new ScpdAction{name = "GetMediaInfo", arguments = [InstanceID, NrTracks, MediaDuration, CurrentURIOut, CurrentURIMetaDataOut, NextURI, NextURIMetaData, PlayMedium, RecordMedium, WriteStatus]},
        ],
        stateVariables = typeof(AvtransportScpd)
            .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(static f => f.FieldType == typeof(StateVariable))
            .Select(f => (StateVariable) f.GetValue(null))
            .ToArray(),
    };

}