## UniProtKB

GCModeller using UniProtKB database as the proteomics analysis engine.

> 请注意，由于数据库更加经常性的执行SELECT操作，而很少执行UPDATE/INSERT/DELETE等更新操作，所以这个数据库之中为了更加快速的查询数据，表之中可能会留存有比较多的冗余字段。并且会在表与表之间存在很复杂的外键链接关系，这是为了尽可能的保证整个知识库的完整性而做出的努力

GCModeller在对蛋白组进行注释分析的时候，主要依赖的是``kb_UniProtKB``数据库之中的注释数据，其中最主要的两个注释数据分别为KEGG的直系同源信息KO注释以及GO注释信息。

|表名称|说明|
|-----|---|
|id_mappings||
|seq_archive|序列归档是以90%的相似度的``UniRef``序列簇为基础所构建的蛋白质参考序列库(http://www.uniprot.org/uniref/?query=&fil=identity:0.9)|
