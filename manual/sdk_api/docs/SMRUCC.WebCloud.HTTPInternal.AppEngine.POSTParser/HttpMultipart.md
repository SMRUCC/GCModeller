# HttpMultipart
_namespace: [SMRUCC.WebCloud.HTTPInternal.AppEngine.POSTParser](./index.md)_

Stream-based multipart handling.

 In this incarnation deals with an HttpInputStream as we are now using
 IntPtr-based streams instead of byte []. In the future, we will also
 send uploads above a certain threshold into the disk (to implement
 limit-less HttpInputFiles).




