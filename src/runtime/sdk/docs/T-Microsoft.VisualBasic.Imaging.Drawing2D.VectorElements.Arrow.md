---
title: Arrow
---

# Arrow
_namespace: [Microsoft.VisualBasic.Imaging.Drawing2D.VectorElements](N-Microsoft.VisualBasic.Imaging.Drawing2D.VectorElements.html)_

按照任意角度旋转的箭头对象



### Methods

#### #ctor
```csharp
Microsoft.VisualBasic.Imaging.Drawing2D.VectorElements.Arrow.#ctor(System.Drawing.Point,System.Drawing.Size,Microsoft.VisualBasic.Imaging.GDIPlusDeviceHandle,System.Drawing.Color)
```


|Parameter Name|Remarks|
|--------------|-------|
|Location|箭头头部的位置|
|Size|高度和宽度|
|GDI|-|
|Color|填充的颜色|


#### InvokeDrawing
```csharp
Microsoft.VisualBasic.Imaging.Drawing2D.VectorElements.Arrow.InvokeDrawing
```
/|_____
 / |
 \ |
 \|-----


### Properties

#### BodyHeightPercentage
箭头的主体部分占据整个高度的百分比
#### HeadLengthPercentage
箭头的头部占据整个长度的百分比
#### Left
忽略了箭头的方向，本箭头对象存粹的在进行图形绘制的时候的左右的位置
#### Right
忽略了箭头的方向，本箭头对象存粹的在进行图形绘制的时候的左右的位置
#### Size
返回图形上面的绘图的大小，而非箭头本身的大小
