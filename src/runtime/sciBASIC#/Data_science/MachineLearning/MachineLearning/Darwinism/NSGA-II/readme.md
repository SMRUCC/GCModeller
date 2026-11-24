NSGA-II（Non-dominated Sorting Genetic Algorithm II，带精英保留策略的快速非支配排序遗传算法）由 Deb 等人在 2002 年提出，是求解**多目标优化问题**最经典、应用最广泛的进化算法之一，其核心思想是通过"快速非支配排序 + 拥挤度比较 + 精英保留"三大机制，在一次运行中同时逼近整个 Pareto 前沿，并让解集在目标空间中均匀分布。
---
## 一、为什么需要 NSGA-II：多目标优化的困境
单目标优化只找"一个最优值"，而现实工程里目标往往是相互冲突的——降低成本可能牺牲性能、缩短工期可能增加风险。**多目标优化不存在唯一最优解，只有一组"谁也不支配谁"的 Pareto 最优解集**：在这些解之间，改进任一目标必然以牺牲另一目标为代价。
判断解的优劣靠**Pareto 支配**关系：设两个解 $x$ 和 $y$（均为最小化问题），若 $x$ 在所有目标上都不差于 $y$，且至少在一个目标上严格优于 $y$，则称 $x$ 支配 $y$，记作 $x \prec y$。所有未被其他解支配的解构成**第一前沿（Pareto front）**。
NSGA-II 的第一代 NSGA 存在三大痛点：非支配排序复杂度高达 $O(MN^3)$、需要手工调节"共享参数 $\sigma_{share}$"来维持多样性、缺乏精英保留导致好解可能丢失。NSGA-II 正是针对这三点做了系统性重构。
---
## 二、算法整体流程
NSGA-II 每一代的迭代逻辑可以概括为下图：
```
开始
 │
 ├─ ① 初始化种群 P₀（随机生成 N 个个体）
 │
 ├─ ② 对 P₀ 做快速非支配排序 → 得到分层 F₁,F₂,...
 │
 └─ 进入主循环 t = 1, 2, ..., T：
       │
       ├─ ③ 二元锦标赛选择（基于 rank + 拥挤度）
       ├─ ④ 模拟二进制交叉 SBX + 多项式变异 → 子代 Qₜ
       ├─ ⑤ 合并 Rₜ = Pₜ ∪ Qₜ  （规模 2N）
       ├─ ⑥ 对 Rₜ 做快速非支配排序 → 分层
       ├─ ⑦ 从 F₁ 开始逐层填充新种群 Pₜ₊₁
       │      └─ 当某层放不下时，按拥挤度从大到小截取
       └─ ⑧ 判断是否满足终止条件
              ├─ 是 → 输出第一前沿
              └─ 否 → 回到③
结束
```
这是 NSGA-II 与传统 GA 的关键差异之一：**父代与子代合并后再统一筛选**，这正是精英保留的实现方式。
---
## 三、三大核心机制详解
### 机制一：快速非支配排序（Fast Non-dominated Sort）
朴素做法需要对所有解两两比较，再反复扫描剩余种群，复杂度为 $O(MN^3)$。NSGA-II 引入两个数据结构把复杂度压到 $O(MN^2)$：
- $S_p$：个体 $p$ **支配**的解的集合
- $n_p$：个体 $p$ **被支配**的次数
第一轮扫描后，所有 $n_p = 0$ 的个体构成第一前沿 $F_1$；把 $F_1$ 从种群中"移除"，并将 $S_p$ 中每个解的 $n_p$ 减一，减到 0 的进入第二前沿 $F_2$，依此类推。每个解对只比较一次，整体代价大幅降低。
### 机制二：拥挤度距离
同一前沿内的个体 rank 相同，无法直接比较。NSGA-II 用**拥挤度距离**衡量某解在目标空间中与邻近解的密集程度：对每个目标函数单独排序，把边界解（该目标的最大、最小值持有者）的距离设为 $+\infty$（保证极值点不被丢弃），中间解的距离等于相邻两个解在该目标上归一化差值之和，最后累加各目标的距离。
$$
d_i = \sum_{m=1}^{M} \frac{f_m^{(i+1)} - f_m^{(i-1)}}{f_m^{\max} - f_m^{\min}}
$$
直观理解：**距离越大代表该个体周围越"空旷"，越值得保留**，从而让 Pareto 前沿分布更均匀，避免种群挤在一个角落。它完全替代了 NSGA-I 需要手工调节的共享参数 $\sigma_{share}$，实现了"无参数"的多样性维持。
### 机制三：精英保留策略
NSGA-I 只从子代中挑选下一代的父本，父代中的优秀个体可能在交叉变异中"蒸发"。NSGA-II 的做法是：**将父代 $P_t$ 和子代 $Q_t$ 合并成 $R_t$（规模 2N），再从中按"前沿等级优先、拥挤度次之"的规则选出 N 个进入下一代**。这样每一代中的最优个体必然存活，显著加快收敛并保证解质量单调不下降。
> **拥挤度比较算子**的选择规则：给定两个个体 $i, j$，若 $rank_i < rank_j$ 则选 $i$；若 rank 相同则选拥挤度更大者。这是锦标赛选择和环境选择共用的统一比较准则。
### 三大机制与 NSGA-I 的对比
| 维度 | NSGA-I | NSGA-II |
|------|--------|---------|
| 非支配排序复杂度 | $O(MN^3)$ | $O(MN^2)$ |
| 多样性维持方式 | 共享函数，需调节 $\sigma_{share}$ | 拥挤度距离，无需额外参数 |
| 精英保留 | 无 | 父子代合并后统一筛选 |
| 收敛性能 | 易退化，好解可能丢失 | 稳定，收敛快 |
| 工程可用性 | 大种群/高维下不可用 | 2~3 目标问题的工业基准 |
---
## 四、参数设置建议
| 参数 | 常见取值 | 作用与调参提示 |
|------|---------|---------------|
| 种群规模 N | 50–200 | 越大前沿分布越均匀，但每代评估代价线性增加 |
| 交叉概率 $p_c$ | 0.8–0.9 | SBX 分布指数 $\eta_c$ 取 10–20，越大子代越接近父代 |
| 变异概率 $p_m$ | $1/D$（D 为决策变量维度） | 多项式变异 $\eta_m$ 取 15–30 |
| 终止代数 | 依问题收敛曲线确定 | 观察 hypervolume 或前沿变化是否饱和 |
---
## 五、常见问题与局限性
- **目标方向必须统一**：NSGA-II 默认全部最小化，若某指标越大越好，传入前需取负号，否则支配关系会算反。
- **前沿分布不均**：通常源于拥挤度在边界解已设为无穷、中层解过度拥挤时被丢——可增大种群规模、调整 SBX 的 $\eta_c$，或在多目标维度超过 3 时改用 NSGA-III。
- **收敛停滞**：常见于交叉变异算子与编码不匹配（如二进制编码解决连续问题）。建议连续变量使用 SBX + 多项式变异，离散变量使用匹配的算子。
- **早熟收敛**：可加大变异概率、引入多样性保留机制，或结合局部搜索形成混合策略。
**最关键的局限在于高维多目标场景**：当目标数超过 3 时，几乎全部个体都会进入第一前沿（支配关系失去区分度），拥挤度距离在高维空间中也不再是可靠的密度估计——NSGA-II 会意外丢弃已经发现的优质解，导致前沿塌陷。这是 NSGA-III 用**参考点方向**替代拥挤度的根本动机。
---
## 六、Python 实现要点
实际工程中推荐直接使用 **pymoo**、**DEAP**、**Platypus** 等成熟框架；若想自行实现，核心模块可拆为四个函数：
```python
import numpy as np
def dominates(a, b):
    """最小化问题：a 支配 b"""
    return np.all(a <= b) and np.any(a < b)
def fast_non_dominated_sort(objs):
    """objs: (N, M)，返回前沿列表 [[idx...], [idx...], ...]"""
    N = len(objs)
    S = [[] for _ in range(N)]       # 每个解支配的解
    n = np.zeros(N, dtype=int)       # 被支配次数
    fronts = [[]]
    for p in range(N):
        for q in range(N):
            if p == q: continue
            if dominates(objs[p], objs[q]):
                S[p].append(q)
            elif dominates(objs[q], objs[p]):
                n[p] += 1
        if n[p] == 0:
            fronts[0].append(p)
    i = 0
    while fronts[i]:
        nxt = []
        for p in fronts[i]:
            for q in S[p]:
                n[q] -= 1
                if n[q] == 0:
                    nxt.append(q)
        i += 1
        fronts.append(nxt)
    fronts.pop()
    return fronts
def crowding_distance(objs, front_idx):
    """对某一前沿内的个体计算拥挤度"""
    N = len(front_idx)
    dist = np.zeros(N)
    for m in range(objs.shape[1]):
        order = np.argsort(objs[front_idx, m])
        dist[order[0]] = dist[order[-1]] = np.inf
        fmin, fmax = objs[front_idx[order[0]], m], objs[front_idx[order[-1]], m]
        if fmax == fmin: continue
        for k in range(1, N - 1):
            dist[order[k]] += (objs[front_idx[order[k+1]], m]
                             - objs[front_idx[order[k-1]], m]) / (fmax - fmin)
    return dist
def environmental_selection(R_objs, N):
    """从合并种群 R 中选出下一代 N 个个体"""
    fronts = fast_non_dominated_sort(R_objs)
    selected, ranks = [], []
    for k, F in enumerate(fronts):
        if len(selected) + len(F) <= N:
            selected.extend(F); ranks.extend([k] * len(F))
        else:                                # 该层放不下，按拥挤度截取
            cd = crowding_distance(R_objs, F)
            pick = np.argsort(-cd)[:N - len(selected)]
            selected.extend([F[i] for i in pick])
            break
    return selected
```
主循环骨架：每代执行"SBX 交叉 + 多项式变异 → 合并父子代 → 环境选择"直到收敛。
---
## 七、典型应用领域与延伸
NSGA-II 已在**工程设计（结构、外形优化）、车间调度、电力系统（水光互补调度、机组组合）、通信（波形优化、基站配置）、土地利用规划、机器学习超参数调优**等领域成为事实上的基准算法。
**选型经验法则**：2–3 目标问题首选 NSGA-II；4 目标以上改用 NSGA-III（基于参考点方向维持多样性）；目标函数平滑且评估昂贵时可尝试 MOPSO；前沿形状规则时 MOEA/D 的分解思想也有优势。两代算法的本质差异可以一句话概括——NSGA-II 用"解之间的距离"维持多样性，NSGA-III 用"解到参考方向的距离"维持多样性，后者在高维目标空间中更不易塌陷。
理解 NSGA-II 的关键，不在于记住流程图，而在于体会它如何把"无序的多目标比较"转化为"分层 + 层内密度"的二维选择准则——这一思想至今仍是大多数 MOEA 的设计骨架。
---
## 八、VB.NET 实现（Darwinism.NSGA2）

本库在 `Darwinism/NSGA-II` 目录下实现了上述算法，命名空间为 `Microsoft.VisualBasic.MachineLearning.Darwinism.NSGA2`（目录名含连字符，不能直接作为命名空间），并完整复用现有 `Chromosome(Of Chr)` / `Population(Of Chr)` 遗传算法框架。

### 8.1 类型一览

| 类型 | 作用 |
|------|------|
| `IMultiObjective(Of Chr)` | 多目标评估接口：`Objectives`（目标个数）、`Cacheable`、`Evaluate(chromosome, parallel) As Double()` |
| `DelegateObjective(Of Chr)` | 用委托函数快速构造多目标评估模型 |
| `SingleObjectiveAdapter(Of Chr)` | 把现有 `Fitness(Of Chr)` 适配为单目标向量（可设置 `maximize`），用于兼容传统 GA |
| `ObjectivePool(Of Chr)` | 按 `Chromosome.Identity` 缓存目标向量，避免重复评估 |
| `ParetoSorting` | `Dominates`、`FastNonDominatedSort`、`CrowdingDistance` 等纯算法函数 |
| `EnvironmentalSelection` | 父子代合并后的精英保留环境选择（rank 优先、拥挤度次之） |
| `NSGA2Engine(Of Chr)` | NSGA-II 引擎：持有种群、执行单代 `Evolve()`、输出 `ParetoFront` |
| `NSGA2Driver(Of Chr)` | 多代循环驱动，继承 `IterationReporter(Of T)`，支持进度回调 |
| `IRealVector(Of Chr)` | 实数向量染色体抽象：`Genes` 与 `Create(genes)` |
| `RealCodedOperators` | SBX 交叉 `SBXCrossover` / 多项式变异 `PolynomialMutation`（含搜索边界与 η 参数） |

### 8.2 快速开始

目标函数返回目标向量，`Chr` 只需实现现有的 `Chromosome(Of Chr)` 接口（沿用 `Crossover` / `Mutate`）：

```vbnet
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF.Population
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.NSGA2

Dim population As New Population(Of MyChromosome)(New PopulationList(Of MyChromosome), parallel:=False) With {
    .capacitySize = 100
}
' 添加 100 个初始个体 ...

' 目标函数：f1, f2 均为最小化目标
Dim objectives As New DelegateObjective(Of MyChromosome)(
    Function(chr, parallel) {Cost(chr), Risk(chr)},
    objectives:=2,
    cacheable:=True
)

Dim engine As New NSGA2Engine(Of MyChromosome)(
    population:=population,
    objectives:=objectives,
    seeds:=Function() myRandom,          ' 可选，默认使用全局随机源
    cacheSize:=1000                       ' 0 表示关闭目标缓存
)

Dim driver As New NSGA2Driver(Of MyChromosome)(
    model:=engine,
    generations:=250
)

Call driver.Train()

For Each solution As ParetoSolution(Of MyChromosome) In engine.ParetoFront
    Console.WriteLine(solution.ToString())
Next
```

### 8.3 兼容单目标 Fitness

```vbnet
' 统一为最小化；原 Fitness 越大越好时传 maximize:=True
Dim objectives As New SingleObjectiveAdapter(Of MyChromosome)(fitnessModel, maximize:=False)
Dim engine As New NSGA2Engine(Of MyChromosome)(population, objectives)
```

单目标时 NSGA-II 会退化为"最优解 + 拥挤度多样性"的选择行为，主要用于平滑迁移已有模型。

### 8.4 实数编码算子

使染色体实现 `IRealVector(Of Chr)`（暴露 `Genes As Double()` 与 `Create(genes) As Chr`），即可使用内置算子：

```vbnet
Dim lower As Double() = Enumerable.Repeat(0.0, 30).ToArray
Dim upper As Double() = Enumerable.Repeat(1.0, 30).ToArray

Dim engine As New NSGA2Engine(Of Zdt1Chromosome)(
    population, objectives,
    seeds:=Function() rnd,
    crossover:=Function(a, b, rand) a.SBXCrossover(b, rand, lower, upper, pc:=0.9, etaC:=20),
    mutate:=Function(chr, rand) chr.PolynomialMutation(rand, lower, upper, rate:=1.0 / 30, etaM:=20)
)
```

也可以直接调用基因级函数 `CrossoverGene` / `MutateGene`，把它们复用到自定义编码中。

### 8.5 参数建议

| 参数 | 建议取值 | 说明 |
|------|---------|------|
| 种群规模 N | 50–200 | 取初始 `Population.Size`，环境选择恰好保留 N 个 |
| `pc`（SBX 交叉概率） | 0.8–0.9 | 未触发交叉时直接返回父本 |
| `etaC`（SBX 分布指数） | 10–20 | 越大子代越接近父代 |
| `pm`（多项式变异概率） | 1/D | 传 `rate<=0` 时回退到 `Chromosome.MutationRate`，仍无效则取 1/D |
| `etaM`（变异分布指数） | 15–30 | 越大扰动越小 |
| `cacheSize` | 0 或 1000 量级 | 按 `Identity` 缓存目标向量，需保证 `Identity` 唯一 |

> 目标方向必须统一为最小化；最大化目标请取负号或使用 `SingleObjectiveAdapter(maximize:=True)`。

### 8.6 示例与验证工程

`Darwinism/NSGA-II/test` 下提供了 ZDT1（30 维、双目标）示例与可执行验证工程：

```
dotnet run --project Darwinism/NSGA-II/test/NSGA2_test.vbproj -c Debug
```

程序固定随机种子运行 `N=100`、`250` 代，并断言：

- 输出前沿两两互不支配；
- 世代距离 `GD <= 0.01`（对解析前沿 `f2 = 1 - sqrt(f1)` 采样计算）；
- `f1` 覆盖范围接近 `[0, 1]`。

验证通过时进程退出码为 `0`，否则为 `1`，可直接用于 CI。