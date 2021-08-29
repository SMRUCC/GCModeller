imports "package_utils" from "devkit";

# require(GCModeller);
package_utils::attach("E:\GCModeller\src\workbench\pkg");

imports "FBA" from "simulators";
imports "repository" from "kegg_kit";

const map = "E:\biodeep\biodeepdb_v3\KEGG\maps\Metabolism\Amino acid metabolism\map00220.XML"
|> repository::load.maps()
;
const objId = as.object(map)$GetMembers();
const reactionId = unique(objId[objId == $"R\d+"]);

print("reaction id list in this map model:");
print(reactionId);

const graph = `E:\biodeep\biodeepdb_v3\KEGG\br08201\mapLinks\${reactionId}.XML`
|> repository::load.reactions
|> FBA::matrix
;

const solution = graph |> FBA::lpsolve;

str(solution);
 

