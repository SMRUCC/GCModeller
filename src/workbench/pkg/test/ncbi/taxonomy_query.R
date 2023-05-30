imports "package_utils" from "devkit";

package_utils::attach(`${@dir}/../../`);

options(http.cache_dir = @dir);

tax = taxonomy_search("Leuconostoc mesenteroides subsp. mesenteroides ATCC 8293");