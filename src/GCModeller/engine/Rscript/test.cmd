@echo off

SET work=S:/synthetic_biology/test_model/wildtype/model
SET output=S:/synthetic_biology/test_model/wildtype/model/test20191127
SET model=%work%\Yersinia_pseudotuberculosis_IP_32953.GCMarkup

REM wildtype test
R# ./run.R --in "%model%" --out "%output%/normals/" --tag normal

REM mutation test
R# ./run.R --in "%model%" --out "%output%/delete_all_DEG/" --tag delete_all_DEG --deletions "%work%/1025_EG.txt"