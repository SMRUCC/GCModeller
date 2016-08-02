**************************************************************************************************************
*Subroutine Fortran pour simulation de données selon un champs aléatoire markovien de type Potts-Dirichlet
**************************************************************************************************************



**************************************************************************************************************
*Cette subroutine donne le  vecteur contenant les indices des points voisins d'un individu ind fixe ainsi 
*que le nombre de voisins de ind

      subroutine nghind(ind,nindiv,matngh,nbrngh,ngh)
*     ind est l'individu d'interet
*     nindiv est le nombre d'individus
*     matngh est la matrice de voisinage des nindiv individus
*     nbrngh est le nombre de voisins de ind
*     ngh est un vecteur contenant les indices des voisins de ind
      implicit none
      integer ind,nindiv,matngh(nindiv,nindiv),nbrngh,
     &     ngh(nindiv)
      integer  imat     

      do imat= 1,nindiv
         if(matngh(ind,imat) .eq. 1) then
            nbrngh= nbrngh+1
            ngh(nbrngh)=imat
         endif
      enddo

      end 




****************************************************************************************************************
*Calcul de la distribution conditionnelle complète (discrète) du label de classe
*de l'individu ind sachant la configuration courante c

      subroutine condition(ind,nindiv,nclass,c,matngh,ngh,
     &           probcond,psi)
      implicit none
      integer ind,nclass,nindiv,c,matngh,ngh
      double precision psi
c     next line commented by Gilles July 18 2008
c      real probcond
      double precision probcond
      dimension  matngh(nindiv,nindiv), probcond(nclass),
     &            c(nindiv),ngh(nindiv)
*variables locales
      integer iclass,n,j,nbrngh
c     next line commented by Gilles July 18 2008
c       real cstnorm
      double precision cstnorm
      nbrngh=0
      cstnorm=0

*Stockage des indices des voisins de ind dans le vecteur ngh
*Stockage du nombre de voisins de ind dans nbrngh
          call nghind(ind,nindiv,matngh,nbrngh,ngh)

          do iclass= 1,nclass
             n=0
*Calcul du nombre de voisins de ind appartenant à chaque population iclass
             do j=1,nbrngh
                if(c(ngh(j)) .eq. iclass) then 
                   n=n+1
                endif   
             enddo
             probcond(iclass)=dexp(psi*dble(n))
             cstnorm= cstnorm+probcond(iclass)
          enddo 
      
         do iclass= 1,nclass
            probcond(iclass)= probcond(iclass)/dble(cstnorm)
         enddo 
      end 





****************************************************************************************************************
*Mise à jour du vecteur c des labels de classe via un pas de Gibbs

      subroutine gibbsup(nindiv,nclass,c,matngh,ngh,
     &     probcond,psi,tempmul)

      implicit none
      integer nclass,nindiv,c,matngh,ngh,tempmul
      double precision psi
c     next line commented by Gilles July 18 2008
c      real probcond
      double precision probcond
      dimension  matngh(nindiv,nindiv),probcond(nclass),
     &           c(nindiv),ngh(nindiv),tempmul(nclass)

*variables locales
      integer iclass,iindiv
   
      do iindiv=1,nindiv

*Calcul  et stockage dans probcond des conditionnelles complètes de chaque individu 
*sachant la configuration c courante   
      
         call condition(iindiv,nindiv,nclass,c,matngh,ngh,probcond,psi)

c     next 3 lines for debugging when call to condition is commented
c         do iclass=1,nclass
c            probcond(iclass) = 1/dble(nclass)
c         enddo
        
c     next line commented by Gilles July 18 2008
c         call genmul(1,probcond,nclass,tempmul)
         call sarmultinom(1.d0,probcond,nclass,tempmul)

         do iclass= 1,nclass
            if(tempmul(iclass) .eq. 1) then
              c(iindiv)= iclass
            endif
         enddo
       
      enddo

      end





******************************************************************************************************************
*Cette fonction calcule l'énergie associée à une configuration Potts des labels de classe des individus 

      integer function energy (nindiv,c,matngh,ngh)
      implicit none
      integer nindiv,c,matngh,nbrngh,ngh
      dimension c(nindiv),matngh(nindiv,nindiv),ngh(nindiv)
      integer iindiv,ingh    
 
      energy=0

      do iindiv=1,nindiv
         nbrngh=0 

         call nghind(iindiv,nindiv,matngh,nbrngh,ngh)

         do ingh=1,nbrngh
            if( c(ngh(ingh)) .eq. c(iindiv) ) then 
               energy=energy+1
            endif
         enddo

      enddo

      energy= energy/2
                 
      end function energy







 


