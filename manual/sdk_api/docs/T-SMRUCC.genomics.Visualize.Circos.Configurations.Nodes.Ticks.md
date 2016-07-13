---
title: Ticks
---

# Ticks
_namespace: [SMRUCC.genomics.Visualize.Circos.Configurations.Nodes](N-SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.html)_






### Properties

#### multiplier
the tick label is derived by multiplying the tick position
 by ``@"P:SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Ticks.multiplier"`` and casting it in ``@"M:Microsoft.VisualBasic.Strings.Format(System.Object,System.String)"``:

 ```
 sprintf(format,position*multiplier)
 ```
