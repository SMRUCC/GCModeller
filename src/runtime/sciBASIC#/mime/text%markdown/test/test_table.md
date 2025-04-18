
1. 正离子模式

| name              | charge | M   | adducts  | mode |
| ----------------- | ------ | --- | -------- | ---- |
| [M]+              | 1      | 1   | ``0``    | ``+``|
| **[M+3H]3+**      | 3      | 1   | 1.00728  | +    |
| _[M+2H+Na]3+_       | 3      | 1   | 8.33459  | +    |
| [M+H+2Na]3+       | 3      | 1   | 15.7662  | +    |
| [M+3Na]3+         | 3      | 1   | 22.9892  | +    |
| [M+2H]2+          | 2      | 1   | 1.00728  | +    |
| [M+H+NH4]2+       | 2      | 1   | 9.52055  | +    |
| [M+H+Na]2+        | 2      | 1   | 11.9982  | +    |
| [M+H+K]2+         | 2      | 1   | 19.9852  | +    |
| [M+ACN+2H]2+      | 2      | 1   | 21.5206  | +    |
| [M+2Na]2+         | 2      | 1   | 22.9892  | +    |
| [M+2ACN+2H]2+     | 2      | 1   | 42.0338  | +    |
| [M+3ACN+2H]2+     | 2      | 1   | 62.5471  | +    |
| [M+H]+            | 1      | 1   | 1.00728  | +    |
| [M+Li]+           | 1      | 1   | 6.941    | +    |
| [M-Cl]+           | 1      | 1   | -35.446  | +    |
| http://aaa.com/?q=__[M-H2O+NH4]+__      | 1      | 1   | 0.023809 | +    |
| [M+H-2H2O]+       | 1      | 1   | -35.0133 | +    |
| [M+H-H2O]+        | 1      | 1   | -17.0027 | +    |
| [M+NH4]+          | 1      | 1   | 18.0338  | +    |
| [M+Na]+           | 1      | 1   | 22.9892  | +    |
| [M+CH3OH+H]+      | 1      | 1   | 33.0335  | +    |
| [M+K]+            | 1      | 1   | 38.9632  | +    |
| [M+ACN+H]+        | 1      | 1   | 42.0338  | +    |
| [M+2Na-H]+        | 1      | 1   | 44.9712  | +    |
| [M+IsoProp+H]+    | 1      | 1   | 61.0653  | +    |
| [M+ACN+Na]+       | 1      | 1   | 64.0158  | +    |
| [M+2K-H]+         | 1      | 1   | 76.919   | +    |
| [M+DMSO+H]+       | 1      | 1   | 79.0212  | +    |
| [M+2ACN+H]+       | 1      | 1   | 83.0604  | +    |
| [M+IsoProp+Na+H]+ | 1      | 1   | 84.0551  | +    |
| [2M+H]+           | 1      | 2   | 1.00728  | +    |
| [2M+NH4]+         | 1      | 2   | 18.0338  | +    |
| [2M+Na]+          | 1      | 2   | 22.9892  | +    |
| [2M+K]+           | 1      | 2   | 38.9632  | +    |
| [2M+ACN+H]+       | 1      | 2   | 42.0338  | +    |
| [2M+ACN+Na]+      | 1      | 2   | 64.0158  | +    |
| [M+H-C12H20O9]+   | 1      | 1   | -307.103 | +    |

#### 2. 负离子模式

| name           | charge | M   | adducts  | mode |
| -------------- | ------ | --- | -------- | ---- |
| [M]-           | -1     | 1   | 0        | -    |
| [M-3H]3-       | -3     | 1   | -1.00728 | -    |
| [M-2H]2-       | -2     | 1   | -1.00728 | -    |
| [M-H2O-H]-     | -1     | 1   | -19.0184 | -    |
| [M-H2O]-       | -1     | 1   | -18.0111 | -    |
| [M-H]-         | -1     | 1   | -1.00728 | -    |
| [M+H]-         | -1     | 1   | 1.00728  | -    |
| [M+Na-2H]-     | -1     | 1   | 20.9747  | -    |
| [M+Cl]-        | -1     | 1   | 34.9694  | -    |
| [M+K-2H]-      | -1     | 1   | 36.9486  | -    |
| [M+FA-H]-      | -1     | 1   | 44.9982  | -    |
| [M+FA]-        | -1     | 1   | 46.0055  | -    |
| [M+Hac-H]-     | -1     | 1   | 59.0139  | -    |
| [M+Br]-        | -1     | 1   | 78.9189  | -    |
| [M+TFA-H]-     | -1     | 1   | 112.986  | -    |
| [M+F]-         | -1     | 1   | 18.998   | -    |
| [2M-H]-        | -1     | 2   | -1.00728 | -    |
| [2M+FA-H]-     | -1     | 2   | 44.9982  | -    |
| [2M+Hac-H]-    | -1     | 2   | 59.0139  | -    |
| [3M-H]-        | -1     | 3   | -1.00728 | -    |
| [M+CH3COO]-    | -1     | 1   | 59.0133  | -    |
| [M+CH3COOH+H]- | -1     | 1   | 61.0284  | -    |
| [M-CH3]-       | -1     | 1   | -15.0235 | -    |
| [M+HCOO]-      | -1     | 1   | 44.9977  | -    |

虽然说，在进行质谱数据分析的时候，``[M+H]+``和``[M-H]-``是非常常见的母离子加合物形式，我们一般将最终的计算结果固定在这两种母离子加合物形式上一般可以正确的得到注释结果，但是在进行母离子的加合物形式计算的时候，我们仍然应该需要根据目标化合物的具体形式来进行计算。例如对于诸如C25H26N9NaO8S2这种带有Na钠盐的化合物，假若我们直接通过减氢的形式计算其在负离子模式下的加合物形式则可能得到错误的结果，因为这种带有Na+离子的化合物在溶液中非常容易丢掉Na+从而直接带上一个单位的负电荷，在这种情况下，加合物的形式应该是``[M-Na]−``，这里的``[M]``代表化合物的分子量，``[M-Na]``则表示分子量减去一个钠离子的原子量（大约是22.99）。