#include <stdlib.h>
#include <math.h>
#include <stdio.h>
#include <time.h>


double distgen(int *d, int *na, double *a, double *ds)
{

int i,j,k,nb;
double *x1, *x2;
/*printf(" c'est parti \n");*/

x1 =  (double *) malloc(*na * sizeof(double));
x2 =  (double *) malloc(*na * sizeof(double));

/*printf(" here we go ! \n");*/

for (i=1; i < *d; i++){
for (k =0; k< *na; k++)  x1[k] = a[ i * (*na) + k ];
for (j=0; j < i ; j++){
nb = 0;
for (k =0; k< *na; k++) {  x2[k] = a[ j * (*na) + k ] ; if ( x1[k] == x2[k] ) nb++;}
for (k =0; k< (*na / 2) ; k++) { if (x1[2*k] == x2[2*k+1]) nb++; if( x1[2*k+1] == x2[2*k]) nb++;}
/**printf("%d %d %d \n", i, j, nb);**/
ds[ j*(*d) + i] = 1.0 - (double) (nb) / (double) ( *na * 2) ;
}}

}
