---
title: DateParseHandling
---

# DateParseHandling
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Specifies how date formatted strings, e.g. "\/Date(1198908717056)\/" and "2012-03-21T05:40Z", are parsed when reading JSON text.




### Properties

#### DateTime
Date formatted strings, e.g. "\/Date(1198908717056)\/" and "2012-03-21T05:40Z", are parsed to @"F:Newtonsoft.Json.DateParseHandling.DateTime".
#### DateTimeOffset
Date formatted strings, e.g. "\/Date(1198908717056)\/" and "2012-03-21T05:40Z", are parsed to @"F:Newtonsoft.Json.DateParseHandling.DateTimeOffset".
#### None
Date formatted strings are not parsed to a date type and are read as strings.
