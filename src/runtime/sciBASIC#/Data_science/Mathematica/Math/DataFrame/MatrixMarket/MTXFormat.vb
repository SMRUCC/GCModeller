﻿''' <summary>
''' ```
''' %%MatrixMarket matrix coordinate real general
''' %=================================================================================
''' %
''' % This ASCII file represents a sparse MxN matrix with L 
''' % nonzeros in the following Matrix Market format:
''' %
''' % +----------------------------------------------+
''' % |%%MatrixMarket matrix coordinate real general | &lt;--- header line
''' % |%                                             | &lt;--+
''' % |% comments                                    |    |-- 0 Or more comment lines
''' % |%                                             | &lt;--+         
''' % |    M  N  L                                   | &lt;--- rows, columns, entries
''' % |    I1  J1  A(I1, J1)                         | &lt;--+
''' % |    I2  J2  A(I2, J2)                         |    |
''' % |    I3  J3  A(I3, J3)                         |    |-- L lines
''' % |        . . .                                 |    |
''' % |    IL JL  A(IL, JL)                          | &lt;--+
''' % +----------------------------------------------+   
''' %
''' % Indices are 1-based, i.e. A(1,1) Is the first element.
''' %
''' %=================================================================================
'''  5  5  8
'''    1     1   1.000e+00
'''    2     2   1.050e+01
'''    3     3   1.500e-02
'''    1     4   6.000e+00
'''    4     2   2.505e+02
'''    4     4  -2.800e+02
'''    4     5   3.332e+01
'''    5     5   1.200e+01 
''' ```
''' </summary>
Public Class MTXFormat

End Class
