require(GCModeller);

print(ecnumber_to_ko(ec_number = [
    "2.7.1.1" "2.7.1.2" "2.7.1.2" "2.7.1.2" "5.3.1.9" "5.3.1.9" 
]));

write.csv(ecnumber_to_ko(), file = file.path(@dir, "kegg_ecnumber.csv"));