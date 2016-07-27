---
title: HttpMultipart
---

# HttpMultipart
_namespace: [SMRUCC.HTTPInternal.AppEngine.POSTParser](N-SMRUCC.HTTPInternal.AppEngine.POSTParser.html)_

Stream-based multipart handling.

 In this incarnation deals with an HttpInputStream as we are now using
 IntPtr-based streams instead of byte []. In the future, we will also
 send uploads above a certain threshold into the disk (to implement
 limit-less HttpInputFiles).




