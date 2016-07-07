---
title: CircosDistributed
---

# CircosDistributed
_namespace: [SMRUCC.genomics.Visualize.Circos.Configurations](N-SMRUCC.genomics.Visualize.Circos.Configurations.html)_

The circos distributed includes files.
 (这个对象仅仅是为了引用Cricos系统内的预置的配置文件的设立的，故而@"M:SMRUCC.genomics.Visualize.Circos.Configurations.CircosDistributed.GenerateDocument(System.Int32)"[
 ]方法和@"M:Microsoft.VisualBasic.ComponentModel.ITextFile.Save(System.String,Microsoft.VisualBasic.TextEncodings.Encodings)"方法可以不会被实现)



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Visualize.Circos.Configurations.CircosDistributed.#ctor(System.String)
```
由于这些是系统的预置的数据，是不能够再修改了的，所以这里由于没有数据配置项，直接忽略掉了Circos配置数据

|Parameter Name|Remarks|
|--------------|-------|
|path|-|



### Properties

#### ColorFontsPatterns
RGB/HSV color definitions, color lists, location of fonts, fill
 patterns. Included from Circos distribution.

 In older versions Of Circos, colors, fonts And patterns were
 included individually. Now, this Is done from a central file. Make
 sure that you're not importing these values twice by having

 ```
 *** Do Not Do THIS ***
 <colors>
 <<include etc/colors.conf>>
 <colors>
 **********************
 ```
#### HouseKeeping
Debugging, I/O an dother system parameters included from Circos distribution.
#### Image
The remaining content Is standard And required. It Is imported from
 Default files In the Circos distribution.

 These should be present In every Circos configuration file And
 overridden As required. To see the content Of these files, 
 look In ``etc/`` In the Circos distribution.

 It's best to include these files using relative paths. This way, the
 files If Not found under your current directory will be drawn from
 the Circos distribution. 

 As always, centralize all your inputs As much As possible.
