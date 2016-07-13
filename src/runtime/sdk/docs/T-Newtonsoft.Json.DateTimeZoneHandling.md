---
title: DateTimeZoneHandling
---

# DateTimeZoneHandling
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Specifies how to treat the time value when converting between string and @"T:System.DateTime".




### Properties

#### Local
Treat as local time. If the @"T:System.DateTime" object represents a Coordinated Universal Time (UTC), it is converted to the local time.
#### RoundtripKind
Time zone information should be preserved when converting.
#### Unspecified
Treat as a local time if a @"T:System.DateTime" is being converted to a string.
 If a string is being converted to @"T:System.DateTime", convert to a local time if a time zone is specified.
#### Utc
Treat as a UTC. If the @"T:System.DateTime" object represents a local time, it is converted to a UTC.
