---
title: Point3D
---

# Point3D
_namespace: [Microsoft.VisualBasic.Imaging.Drawing3D](N-Microsoft.VisualBasic.Imaging.Drawing3D.html)_

Defines the Point3D class that represents points in 3D space.
 Developed by leonelmachava <leonelmachava@gmail.com>
 http://codentronix.com

 Copyright (c) 2011 Leonel Machava



### Methods

#### Project
```csharp
Microsoft.VisualBasic.Imaging.Drawing3D.Point3D.Project(System.Int32,System.Int32,System.Int32,System.Int32)
```
将3D投影为2D，所以只需要取结果之中的@"P:Microsoft.VisualBasic.Imaging.Drawing3D.Point3D.X"和@"P:Microsoft.VisualBasic.Imaging.Drawing3D.Point3D.Y"就行了

|Parameter Name|Remarks|
|--------------|-------|
|viewWidth|-|
|viewHeight|-|
|fov|256默认值|
|viewDistance|-|


#### RotateX
```csharp
Microsoft.VisualBasic.Imaging.Drawing3D.Point3D.RotateX(System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|angle|度，函数里面会自动转换为三角函数所需要的弧度的|



