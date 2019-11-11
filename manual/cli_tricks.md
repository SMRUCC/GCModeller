# GCModeller CLI Cheat Sheet

## 1. Text grep scripting

The text grep scripting feature is usually used in blast output parser cli, there are some command that you could using in the GCModeller cli scripting:

+ ``tokens``
+ ``match``
+ ``-``
+ ``replace``
+ ``mid``
+ ``reverse``

## 2. Graphics engine configuration

The gdi+ graphics engine is used in GCModeller plot system, by default. But you can change the graphics engine from command when going to run a chart plot api from GCModeller cli tools, example cli as:

```
tool /command [...arguments] /@set "graphic_driver=svg"
```