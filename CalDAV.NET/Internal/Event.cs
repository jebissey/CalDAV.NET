using CalDAV.NET.Enums;
using CalDAV.NET.Interfaces;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using System;

namespace CalDAV.NET.Internal;

internal class Event : IEvent
{
    private static readonly CalendarSerializer _calendarSerializer = new CalendarSerializer();

    public EventState Status { get; set; }
    public string Uid => _calendarEvent.Uid;
    public DateTime Created => _calendarEvent.Created.Value;
    public DateTime LastModified => _calendarEvent.LastModified.Value;

    public DateTime Start
    {
        get => _calendarEvent.DtStart.Value;
        set => _calendarEvent.DtStart = SetDateTime(_calendarEvent.DtStart, value);
    }

    public DateTime End
    {
        get => _calendarEvent.DtEnd.Value;
        set => _calendarEvent.DtEnd = SetDateTime(_calendarEvent.DtEnd, value);
    }

    public TimeSpan Duration
    {
        get => _calendarEvent.Duration;
        set
        {
            _calendarEvent.Duration = value;

            Changed();
        }
    }

    public DateTime Stamp
    {
        get => _calendarEvent.DtStamp.Value;
        set => _calendarEvent.DtStamp = SetDateTime(_calendarEvent.DtStamp, value);
    }

    public string Location
    {
        get => _calendarEvent.Location;
        set
        {
            _calendarEvent.Location = value;

            Changed();
        }
    }

    public string Summary
    {
        get => _calendarEvent.Summary;
        set
        {
            _calendarEvent.Summary = value;

            Changed();
        }
    }

    private readonly CalendarEvent _calendarEvent;

    public Event(CalendarEvent calendarEvent)
    {
        _calendarEvent = calendarEvent;
        Status = EventState.None;
    }

    public override string ToString()
    {
        var content = Summary;

        if (string.IsNullOrEmpty(Location) == false)
        {
            content += $" at {Location}";
        }

        return $"{content} on {Start} till {End}";
    }

    internal string Serialize(Ical.Net.Calendar calendar)
    {
        calendar.Events.Clear();
        calendar.Events.Add(_calendarEvent);

        return _calendarSerializer.SerializeToString(calendar);
    }

    private void Changed()
    {
        // Do not care about changes if flagged for deletion
        if (Status == EventState.Deleted)
        {
            return;
        }

        Status = EventState.Changed;
    }

    private IDateTime SetDateTime(IDateTime target, DateTime value)
    {
        if (target?.Value == value)
        {
            return target;
        }

        Changed();

        if (target == null)
        {
            return new CalDateTime(value);
        }

        target.Value = value;

        return target;
    }
}
