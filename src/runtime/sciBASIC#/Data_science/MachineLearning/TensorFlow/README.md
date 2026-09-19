# 纯 VB.NET 张量库（NumPy 风格 API 与自动微分）

## 引言

**首先要澄清一件事**：本包的名字虽然叫 `TensorFlow`，但它**不是** Google TensorFlow 的绑定，而是一个**自包含的托管数值引擎**。

它解决的是 .NET 生态的一个真实缺口：需要多维数组与自动微分，但不想引入几百 MB 的原生运行时。

## 设计目标

- **零原生依赖**：纯托管实现，可单文件部署；
- **NumPy 风格 API**：降低从 Python 数值生态迁移的成本；
- **自动微分**：让梯度从表达式自动产生，而非手写反向传播。

## 核心能力

| 命名空间 | 能力 |
|---|---|
| `...MachineLearning.TensorFlow`（根） | `Tensor`：托管多维数组，支持切片、广播与算术运算 |
| `....TensorFlow.NumPy` | NumPy 风格 API（`np` 式的数组操作惯例） |
| `....TensorFlow.Compute` | 张量计算引擎 |
| `....TensorFlow.AutomaticDifferentiation`（+ `Parallel`） | **反向模式自动微分**（含并行变体） |
| `....TensorFlow.Math` / `nn` | 数值运算、激活函数与损失函数 |

## 快速上手

```vbnet
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

' 1. 创建张量（NumPy 风格）
Dim a As Tensor = np.RandomNormal(shape:={100, 50})
Dim b As Tensor = np.Ones(shape:={50, 20})

' 2. 常规运算（广播、切片、矩阵乘）
Dim c As Tensor = a.MatMul(b)
Dim column = a(All, 0)

' 3. 自动微分：定义前向表达式即可得到梯度
Dim loss As Func(Of Tensor, Tensor) = Function(w) np.Sum((a.MatMul(w) - target) ^ 2)
Dim grad = AutomaticDifferentiation.Gradient(loss, w)

' 4. 神经网络常用构件
Dim activated = nn.Relu(c)
Dim l = nn.MeanSquaredError(c, target)
```

## 实现要点

- **反向模式自动微分为什么是深度学习的基石**：它让「求损失对所有参数的梯度」只需**一次**反向遍历计算图，成本与前向传播同量级。若用数值微分，参数量有多少就要前向多少次。
- **并行变体的意义**：大张量的逐元素运算天然可并行；`Parallel` 变体把元素级操作分块到多个线程，在多核机器上获得近似线性加速。
- **托管实现的取舍**：缺少 SIMD/GPU 级别的底层优化，因此大模型训练性能不及原生框架；但作为**可调试、可嵌入、零部署成本**的数值引擎，它对教学、小模型与嵌入式场景非常合适。
- **与 `ILCudaTensor` 的关系**：`ILCudaTensor` 实现了本包定义的 `ITensorCompute` 接口，并继承 `TensorComputeBase` 作为 CPU 兜底——因此在本包之上注册一次 CUDA 后端，所有 Tensor / Math / nn 运算即可透明地切到 GPU。

## 包信息

- Assembly：`Microsoft.VisualBasic.MachineLearning.TensorFlow`
- TargetFramework：`net10.0`
- Tags：`scibasic;tensor;numpy;automatic-differentiation;reverse-mode;numerical;computation-engine;activation-functions`
- 许可：GPL-3.0-or-later
