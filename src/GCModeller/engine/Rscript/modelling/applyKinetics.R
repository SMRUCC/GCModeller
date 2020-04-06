imports "vcellkit.modeller" from "vcellkit";

let modelFile as string = "K:\20200226\metabolism\vcell\model.Xml";
let cacheData = "K:\20200226\metabolism\vcell\kinetics\.cache";
let model <- modelFile
:> read.vcell
:> apply.kinetics(cache = cacheData)
;

model
:> xml
:> writeLines(con = `${dirname(modelFile)}/${basename(modelFile)}.applyKinetics.XML`)
;

zip(model, "K:\20200226\metabolism\vcell\vcellModel.zip");