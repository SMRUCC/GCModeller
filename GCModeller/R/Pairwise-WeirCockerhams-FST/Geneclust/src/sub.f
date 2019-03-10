*********************************************************************************************
*Subroutines Fortran
*********************************************************************************************

*Limites du rectangle contenant les coordonnees

      subroutine limit(nindiv,s,xlim,ylim,dt)
      implicit none
      integer nindiv
      double precision s(nindiv,2),xlim(2),ylim(2),dt
      integer iindiv
      xlim(1) = 1.e+30
      xlim(2) = -1.e+30
      ylim(1) = 1.e+30
      ylim(2) = -1.e+30
      do iindiv=1,nindiv
         xlim(1) = dmin1(s(iindiv,1),xlim(1))
         xlim(2) = dmax1(s(iindiv,1),xlim(2))
         ylim(1) = dmin1(s(iindiv,2),ylim(1))
         ylim(2) = dmax1(s(iindiv,2),ylim(2))
      enddo
      xlim(1) = xlim(1) - dt*.5
      xlim(2) = xlim(2) + dt*.5
      ylim(1) = ylim(1) - dt*.5
      ylim(2) = ylim(2) + dt*.5
      end



*********************************************************************************************
*Simulation d'une Dirichlet(1,...,1)
*(p(1),...,p(k)) uniforme dans l'ensemble {p(1)+...+p(k)=1}

      subroutine dirichlet1(nall,nallmax,p)
      implicit none
      integer nall,nallmax
      double precision p(nallmax)
      integer i
      double precision s,ggrexp
      s = 0.
      do i=1,nall
         p(i) = ggrexp(1.d0)
         s = s + p(i)
      enddo
      do i=1,nall
         p(i) =  p(i)/s
      enddo
      if(nallmax .gt. nall) then
         do i=nall+1,nallmax
            p(i) =  -1
         enddo
      endif
      end




**********************************************************************************************
*Simulation d'une Dirichlet(a1,...,an)

      subroutine dirichlet(n,nmax,a,p)
      implicit none
      integer n,nmax
      double precision a(nmax),p(nmax)
      integer i
      double precision s,ggrgam
      s = 0.
      do i=1,n
         p(i) = 0.
         do while(p(i) .lt. 1e-38) 
            p(i) = ggrgam(a(i),1.d0)
         enddo
         s = s + p(i)
      enddo
      do i=1,n
         p(i) =  p(i)/s
      enddo
      if(nmax .gt. n) then
         do i=n+1,nmax
            p(i) =  -1
         enddo
      endif
      end




************************************************************************************************
*Calcul de la log-vraisemblance du modèle 

      double precision function loglikelihood(z,f,c,fis,nclass,nall,
     &      nallmax,nindiv,nloc,nloc2)
      implicit none
      integer nclass,nall,nallmax,nindiv,nloc,nloc2,
     &     z,c
      double precision f,fis,ftemp
      dimension  z(nindiv,nloc2),c(nindiv),f(nclass,nloc,nallmax),
     &      fis(nclass)
*     variables locales
      integer iindiv,iloc,iall1,iall2,iclass


      loglikelihood= 0.

      do iindiv=1,nindiv
           iclass = c(iindiv) 
           do iloc=1,nloc
                iall1 = z(iindiv,2*iloc-1)
                iall2 = z(iindiv,2*iloc)
                if(iall1 .eq. iall2) then  
                       loglikelihood= loglikelihood
     &             + log( f(iclass,iloc,iall1)**2 + 
     &                    (fis(iclass)*f(iclass,iloc,iall1)*
     &                       (1-f(iclass,iloc,iall1))))
                endif     
                if(iall1 .ne. iall2) then
                       loglikelihood= loglikelihood
     &             + log( f(iclass,iloc,iall1)*
     &                     f(iclass,iloc,iall2)*(1-fis(iclass)))                       
                endif
           enddo
      enddo 
      end 




****************************************************************************************************
*Calcul du ratio de vraisemblance p(z|f*)/p(z|f) pour la mise à jour des fréquences alléliques

      double precision function ratiof(z,f,ftemp,c,fis,
     &     nclass,nall,nallmax,nindiv,nloc,nloc2)
      implicit none
      integer nclass,nall,nallmax,nindiv,nloc,nloc2,
     &     z,c
      double precision f,fis,ftemp
      dimension  z(nindiv,nloc2),c(nindiv),
     &     f(nclass,nloc,nallmax),ftemp(nclass,nloc,nallmax),fis(nclass)
*     variables locales
      integer iindiv,iloc,iall1,iall2,iclass
      double precision num,den

c$$$      write(*,*) 'debut de ratiof'
c$$$      write(*,*) 'f=',f
c$$$      write(*,*) 'ftemp=',ftemp

      ratiof = 1.


      do iindiv=1,nindiv

           iclass = c(iindiv) 

           do iloc=1,nloc
c$$$            write(*,*) 'iloc=',iloc
c$$$            write(6,*) 'z=',z(iindiv,2*iloc-1)
c$$$            write(6,*) 'z=',z(iindiv,2*iloc)

                iall1 = z(iindiv,2*iloc-1)
                iall2 = z(iindiv,2*iloc)

c$$$            write(*,*) iall1
c$$$            write(*,*) iall2

                 if(iall1 .eq. iall2) then           
                        num = ftemp(iclass,iloc,iall1)**2 + 
     &                    (fis(iclass)*ftemp(iclass,iloc,iall1)*
     &                       (1-ftemp(iclass,iloc,iall1)))
                        den = f(iclass,iloc,iall1)**2 + 
     &                    (fis(iclass)*f(iclass,iloc,iall1)*
     &                       (1-f(iclass,iloc,iall1)))
                        ratiof = ratiof*(num/den)         
                 endif

                 if(iall1 .ne. iall2) then
                        num = ftemp(iclass,iloc,iall1)*
     &                     ftemp(iclass,iloc,iall2)*(1-fis(iclass))
                        den = f(iclass,iloc,iall1)*
     &                     f(iclass,iloc,iall2)*(1-fis(iclass))
                        ratiof = ratiof*(num/den)
                  endif 
            enddo
      enddo
c$$$      write(*,*) 'fin  de ratiof'
      end function ratiof





****************************************************************************************************************
*Mise a jour de f via Metropolis-Hastings
*prior p(f) Dirichlet(1,...,1) 

      subroutine rpostf(c,z,fis,f,ftemp,nclass,nindiv,nloc,nloc2,
     &             nall,nallmax)
      implicit none 
      integer nclass,nloc,nloc2,nall,nallmax,nindiv,c,z
      double precision f,ftemp,fis
      dimension nall(nloc), c(nindiv),
     &                z(nindiv,nloc2),f(nclass,nloc,nallmax),
     &                ftemp(nclass,nloc,nallmax),fis(nclass)
*variables locales
      integer  iclass,iloc,iindiv,iall,all1,all2
      double precision a,ggrbet,u,ggrunif,ratiof,ratiomh
  
   
      do iclass= 1,nclass
         do iloc= 1,nloc
             do iall=1,nall(iloc)
                 ftemp(iclass,iloc,iall)=f(iclass,iloc,iall)
             enddo
         enddo
      enddo


      do iclass= 1,nclass
           do iloc= 1,nloc

                all1=0
                all2=0

*Choix aléatoires de deux allèles
                do while (all1 .eq. all2)
                    all1= 1+int(aint(float(nall(iloc))*
     &                     ggrunif(0.d0,1.d0)))
                    all2= 1+int(aint(float(nall(iloc))*
     &                     ggrunif(0.d0,1.d0)))
                 enddo

                 a=1
  
                 do iall= 1,nall(iloc)
                     if(iall .ne. all1) then
                        if(iall .ne. all2) then
                            a= a-f(iclass,iloc,iall)
                        endif
                     endif
                  enddo


*Proposition pour les frequences alleliques des allèles all1 et all2 pour iclass et iloc                                      
                  ftemp(iclass,iloc,all1)= a*ggrbet(2.d0,2.d0)
                  ftemp(iclass,iloc,all2)= a-ftemp(iclass,iloc,all1)

*Calcul du ratio de Metropolis-Hastings
                  ratiomh = ratiof(z,f,ftemp,c,fis,nclass,nall,nallmax,
     &                    nindiv,nloc,nloc2)

c$$$                  write(*,*) 'rapportvrais',ratiomh

                  ratiomh = ratiomh*(f(iclass,iloc,all1)/ftemp(iclass,
     &                    iloc,all1))
                  ratiomh = ratiomh*((1-f(iclass,iloc,all1))/
     &                   (1-ftemp(iclass,iloc,all1)))                                                                                                                                                       
c$$$                   write(*,*) 'ratio',ratiomh
            
*Acceptation/Rejet
                   u = ggrunif(0.d0,1.d0)
                   if (u .le. ratiomh) then
                         f(iclass,iloc,all1)=ftemp(iclass,iloc,all1)
                         f(iclass,iloc,all2)=ftemp(iclass,iloc,all2)
                    else
                         ftemp(iclass,iloc,all1) = f(iclass,iloc,all1)
                         ftemp(iclass,iloc,all2) = f(iclass,iloc,all2)
                    endif
        enddo
      enddo

      end subroutine rpostf





*********************************************************************************************************
*Calcul de la variation d'energie du systeme lors de la mise à jour de c

      subroutine caldelta(indup,nindiv,c,ctemp,matngh,
     &           nbrnghup,nghup,delta)
*indup est l'indice de l'individu mis a jour
*nindiv est le nombre d'individus
*c est le vecteur courant des labels de classe 
*ctemp est le vecteur proposé des labels de classe 
*matngh est la matrice de voisinage des nindiv individus
*nbrnghup est le nombre de  voisins de indup
*nghup vecteur contenant les indices des voisins de indup 
      implicit none
      integer indup,nindiv,c(nindiv),ctemp(nindiv),
     &            matngh(nindiv,nindiv),nghup(nindiv),
     &            nbrnghup
      integer N1,N2,j,delta
        
*Stockage des indices des voisins de indup dans le vecteur nghup
*Stockage du nombre de voisins de indup dans nbrnghup
      call nghind(indup,nindiv,matngh,nbrnghup,nghup)

*compt1= compteur du nombre de voisins de indup dont 
*le label de classe est le même que le label proposé pour indup 
      N1=0

*compt2= compteur du nombre de voisins de indup dont 
*le label de classe est le même que le label courant pour indup 
      N2=0


      do j=1,nbrnghup
         if(ctemp(nghup(j)) .eq. ctemp(indup)) then
                  N1= N1+1
         endif
         if(c(nghup(j)) .eq. c(indup)) then
                  N2= N2+1
         endif
      enddo
 
      delta= N1-N2

      end subroutine caldelta
                 




*****************************************************************************************************
*Calcul du ratio de vraisemblance p(z|c*)/p(z|c) pour la mise à jour de c 

      double precision function ratioc(z,f,c,ctemp,fis,
     &     nclass,nall,nallmax,nindiv,nloc,nloc2)
      implicit none
      integer nclass,nall,nallmax,nindiv,nloc,nloc2,
     &     z,c,ctemp
      double precision f,fis
      dimension  z(nindiv,nloc2),c(nindiv),ctemp(nindiv),
     &     f(nclass,nloc,nallmax),fis(nclass)
*     variables locales
      integer iindiv,iloc,iall1,iall2,iclass,iclasstemp
      double precision num, den

c      write(*,*) 'debut de ratioc'
c      write(*,*) 'c=',c
c      write(*,*) 'ctemp=',ctemp


      ratioc = 1.
      do iindiv=1,nindiv
c           write(*,*) 'iindiv=', iindiv
           iclass = c(iindiv)
           iclasstemp = ctemp(iindiv)
C          write(*,*) 'c=',c
C          write(*,*) 'ctemp=',ctemp
C          write(*,*) 'iclass=',iclass
C          write(*,*) 'iclasstemp=',iclasstemp

           do iloc=1,nloc
c               write(*,*) 'iloc=',iloc
c               write(6,*) 'z=',z(iindiv,2*iloc-1)
c               write(6,*) 'z=',z(iindiv,2*iloc)
               iall1 = z(iindiv,2*iloc-1)
               iall2 = z(iindiv,2*iloc)
c              ratioc = ratioc*
c     &              (f(iclasstemp,iloc,iall1)/f(iclass,iloc,iall1))*
c     &              (f(iclasstemp,iloc,iall2)/f(iclass,iloc,iall2))
               if(iall1 .eq. iall2) then 
                      num = f(iclasstemp,iloc,iall1)**2 + 
     &                    fis(iclasstemp)*f(iclasstemp,iloc,iall1)*
     &                       (1-f(iclasstemp,iloc,iall1))
                      den = f(iclass,iloc,iall1)**2 + 
     &                    fis(iclass)*f(iclass,iloc,iall1)*
     &                       (1-f(iclass,iloc,iall1))
                      ratioc = ratioc*(num/den)               
                endif
                if(iall1 .ne. iall2) then 
                      num = f(iclasstemp,iloc,iall1)* 
     &                      f(iclasstemp,iloc,iall2)*(1-fis(iclasstemp))
                      den = f(iclass,iloc,iall1)*
     &                      f(iclass,iloc,iall2)*(1-fis(iclass))
                      ratioc = ratioc*(num/den)   
                 endif      
            enddo
      enddo
c      write(*,*) 'fin de ratioc'
      end function ratioc





*****************************************************************************************************
*Mise a jour de c
*Prior p(c) Modèle de Potts de parametre psi
*Pas de Metropolis-Hastings avec proposal indépendant
*Proposal= uniforme sur (1,2,....,nclass)

      subroutine rpostc(c,ctemp,z,nindiv,nloc,
     &      nloc2,nall,nallmax,nclass,f,matngh,psi,
     &      fis,nghup)
      implicit none
      integer nindiv,nloc,nloc2,delta,
     &      nall,nallmax,nclass,c,ctemp,
     &      z,matngh,nghup,nbrnghup
      double precision psi,f,ratioc,u,ggrunif,ratiomh,fis
      integer iindiv
      dimension nall(nloc),c(nindiv),ctemp(nindiv),
     &     z(nindiv,nloc2),matngh(nindiv,nindiv),
     &     nghup(nindiv),f(nclass,nloc,nallmax),fis(nclass)

      do iindiv= 1,nindiv
         ctemp(iindiv) = c(iindiv)
      enddo


      do iindiv=1,nindiv

         delta=0
         nbrnghup=0

*Proposition pour le label de classe de indiv
         ctemp(iindiv) = 1 + int(aint(float(nclass)*ggrunif(0.d0,1.d0)))

*Calcul du ratio de Metropolis-Hastings
        ratiomh = ratioc(z,f,c,ctemp,fis,nclass,nall,nallmax,nindiv,
     &       nloc,nloc2)
c$$$        write(*,*) 'rapportvrais',ratiomh
*Calcul de la variation d'énergie du systeme 
        call caldelta(iindiv,nindiv,c,ctemp,matngh,nbrnghup,nghup,delta)
        ratiomh= ratiomh*exp(psi*delta)
c$$$    write(*,*) 'ratio update c=',ratiomh
*       write(*,*)''

*Acceptation/Rejet
          u = ggrunif(0.d0,1.d0)
          if (u .le. ratiomh) then
c             write(*,*)'update'
             c(iindiv)=ctemp(iindiv)
          else
             ctemp(iindiv) = c(iindiv)
         endif
      enddo

      end subroutine rpostc





**************************************************************************************************
*Mise a jour de psi
*Prior p(psi) = loi discrète  (Attention! p(psi=k).neq.0) pour tout k)
*Pas de Metropolis-Hastings avec pour proposal une marche aleatoire de +ou-0.1


      subroutine rpostpsi(nindiv,c,psi,tabcst,psimax,
     &                      numpsi,matngh,ngh,priorpsi)

      implicit none 
      integer nindiv,c,matngh,ngh,numpsi
      double precision psi,psimax,tabcst,priorpsi
      dimension c(nindiv),matngh(nindiv,nindiv),tabcst(numpsi),
     &                ngh(nindiv), priorpsi(numpsi)

*Variables temporaire
      integer energyb,energy,indpsi,indpsitemp
      double precision ggrunif,psitemp,u,v,diffcst,num,den,
     &     ratioprior, ratiopost, ratiomh


*Proposition
      u=ggrunif(0.d0,1.d0)
      if (u .le. 0.5) then 
         psitemp= psi-0.1
      else 
         psitemp= psi+0.1
      endif
 
     

*Calcul du ratio de Metropolis-Hastings
      
      ratiomh=0

      if(psitemp .ge. 0) then
           if(psitemp .le. psimax) then
*ATTENTION! Seulement valable pour steppsi=0.1
              indpsi= int((psi*10)+1)
              indpsitemp= int((psitemp*10)+1)

              diffcst= tabcst(indpsi)- tabcst(indpsitemp)
              energyb= energy(nindiv,c,matngh,ngh)
              ratiopost =  exp(energyb*(psitemp-psi)+diffcst)             
              ratioprior = priorpsi(indpsitemp)/priorpsi(indpsi)
              
              ratiomh= ratiopost*ratioprior
           endif                
       endif
           
*      write(*,*) ratiomh

*Acceptation/rejet
       
       v=ggrunif(0.d0,1.d0)
    
       if(v .le. ratiomh) then
            psi=psitemp
       endif

      
                           
       end subroutine rpostpsi





******************************************************************************************************
*Calcul du ratio de vraisemblance p(z|fis*)/p(z|fis) pour la mise à jour de fis 

      double precision function ratiofis(z,f,c,fis,fistemp,
     &     nclass,nall,nallmax,nindiv,nloc,nloc2)
      implicit none
      integer nclass,nall,nallmax,nindiv,nloc,nloc2,
     &     z,c
      double precision f,fis,fistemp
      dimension  z(nindiv,nloc2),c(nindiv),
     &     f(nclass,nloc,nallmax),fis(nclass),fistemp(nclass)
*     variables locales
      integer iindiv,iloc,iall1,iall2,iclass
      double precision num, den

c      write(*,*) 'debut de ratiofis'
c      write(*,*) 'fis=',fis
c      write(*,*) 'fistemp=',fistemp


      ratiofis = 1.

      do iindiv=1,nindiv
c           write(*,*) 'iindiv=', iindiv
           iclass = c(iindiv)
  
C          write(*,*) 'c=',c
C          write(*,*) 'iclass=',iclass

           do iloc=1,nloc
c               write(*,*) 'iloc=',iloc
c               write(6,*) 'z=',z(iindiv,2*iloc-1)
c               write(6,*) 'z=',z(iindiv,2*iloc)
                iall1 = z(iindiv,2*iloc-1)
                iall2 = z(iindiv,2*iloc)
                if(iall1 .eq. iall2) then
                      num = f(iclass,iloc,iall1)**2 + 
     &                    (fistemp(iclass)*f(iclass,iloc,iall1)*
     &                       (1-f(iclass,iloc,iall1)))
                      den = f(iclass,iloc,iall1)**2 + 
     &                    (fis(iclass)*f(iclass,iloc,iall1)*
     &                       (1-f(iclass,iloc,iall1)))
                      ratiofis = ratiofis*(num/den)           
                endif
                if(iall1 .ne. iall2) then 
                      num = f(iclass,iloc,iall1)* 
     &                      f(iclass,iloc,iall2)*(1-fistemp(iclass))
                      den = f(iclass,iloc,iall1)*
     &                      f(iclass,iloc,iall2)*(1-fis(iclass))
                      ratiofis = ratiofis*(num/den)   
                 endif      
            enddo
      enddo
c      write(*,*) 'fin de ratiofis'
      end function ratiofis





*************************************************************************************************************
*Mise à jour des coefficients de consanguinité (successivement pour chaque population)
*Pas de Métropolis-Hastings avec proposal indépendant uniforme sur [0,1]

       subroutine rpostfis(c,z,fis,fistemp,f,nclass,nindiv,nloc,nloc2,
     &             nall,nallmax,alpha,beta)
       implicit none
       integer c,z,nclass,nindiv,nloc,nloc2,nall,nallmax
       double precision fis,fistemp,f,alpha,beta
       dimension c(nindiv),z(nindiv,nloc2),fis(nclass),fistemp(nclass),
     &            f(nclass,nloc,nallmax)
*Variables locales
       integer iclass
       double precision ratiomh,ratiofis,ggrunif,num,den,u


       do iclass=1,nclass
             fistemp(iclass)=fis(iclass)
       enddo


       do iclass=1,nclass

*Proposition pour le coefficient de consanguinité de la pop iclass
*Tirage dans une uniforme sur [0,1]
             fistemp(iclass) = ggrunif(0.d0,1.d0)
          
*Calcul du ratio de Metropolis-Hastings
             ratiomh = ratiofis(z,f,c,fis,fistemp,
     &              nclass,nall,nallmax,nindiv,nloc,nloc2)
c$$$         write(*,*) 'rapportvrais',ratiomh

*Calcul du ratio des priors  B(alpha,beta)
             num= (fistemp(iclass)**(alpha-1))*
     &              ((1-fistemp(iclass))**(beta-1))
             den= (fis(iclass)**(alpha-1))*
     &              ((1-fis(iclass))**(beta-1))
             ratiomh= ratiomh*(num/den)        
c$$$         write(*,*) 'ratio update fis(iclass)=',ratiomh
*            write(*,*)''


*Acceptation/Rejet
            u = ggrunif(0.d0,1.d0)
            if (u .le. ratiomh) then
c                  write(*,*)'update'
                   fis(iclass)=fistemp(iclass)
            else
                   fistemp(iclass)=fis(iclass)
            endif

      enddo

      end subroutine rpostfis




***************************************************************************************************
*     Calcul du Fst d'apres programme en Turbo Pascal d'Arnaud Estoup
*     le vecteur c contient la variable de classe
*     les effectifs des classes doivent etre donnees en entree

      subroutine fstae (nindiv,nppmax,nloc,nloc2,nall,nclass,effcl,z,c,
     &     tabindiv,kk,Fistot,Fsttot,Fittot)
      implicit none
      integer nindiv,nppmax,nloc,nloc2,nall(nloc),nclass,
     &     z(nindiv,nloc2),c(nppmax),effcl(nclass)
      double precision Fsttot,Fittot,Fistot
      integer iloc,iclass,iall,iindiv
      integer g1,g2,k,tabindiv(nindiv,nclass),kk(nclass),ni
      double precision   s1,s2,s3,s1l,s2l,s3l,sni,sni2,sniA,
     &     sniAA,s2A,nA,AA,
     &     nc,MSG,MSI,MSP,s2G,s2I,s2P,Fst,Fit,Fis

*     Recherche des indices des indiv de chaque classe 
      do iclass=1,nclass
         kk(iclass) = 1
      enddo
      do iindiv=1,nindiv
         tabindiv(kk(c(iindiv)),c(iindiv)) = iindiv
         kk(c(iindiv)) = kk(c(iindiv)) + 1
      enddo
c      write(*,*) (tabindiv(iindiv,1),iindiv=1,nindiv)
c      write(*,*) (tabindiv(iindiv,2),iindiv=1,nindiv)
       
      s1 = 0.d0
      s2 = 0.d0
      s3 = 0.d0
      do iloc=1,nloc
c         write(*,*) 'iloc=', iloc
         s1l = 0.d0
         s2l = 0.d0
         s3l = 0.d0
         do iall=1,nall(iloc)
c            write(*,*) 'iall=', iall
            sni = 0.d0
            sni2 = 0.d0
            sniA = 0.d0
            sniAA = 0.d0
            s2A = 0.d0
c            k = 1
            do iclass=1,nclass
c               write(*,*) 'iclass=',iclass
               ni = effcl(iclass)
c                write(*,*) 'ni=',ni
               nA = 0.d0
               AA = 0.d0
c               do iindiv=k,(k+ni-1)
               do iindiv=1,ni
                  k = tabindiv(iindiv,iclass)
c                  write(*,*) 'iindiv=',iindiv
                  g1 = z(k,2*(iloc-1)+1)
                  g2 = z(k,2*(iloc-1)+2)
                  if((g1 .eq. iall) .and. (g2 .eq. iall)) then 
                     AA = AA + 1.d0
                  endif
                  if(g1 .eq. iall) then 
                     nA = nA + 1.d0 
                  endif
                  if(g2 .eq. iall) then 
                     nA = nA + 1.d0 
                  endif
c                  write(*,*) 'variables=',AA,nA
               enddo 
c               k = k+ni
               sniA = sniA + nA
               sniAA = sniAA + AA
               sni = sni + ni 
               sni2 = sni2 + ni*ni
               s2A = s2A + nA*nA/(2*ni)
c               write(*,*) 'variables=',sniA,sniAA,sni,sni2,s2A
            enddo
            nc = (sni-sni2/sni)/(nclass-1)
            MSG = (0.5*sniA-sniAA)/sni
            MSI = (0.5*sniA+sniAA-s2A)/(sni-nclass)
            MSP = (s2A-0.5*(sniA**2)/sni)/(nclass-1)
            s2G = MSG
            s2I = 0.5*(MSI-MSG)
            s2P = (MSP-MSI)/(2*nc)
            s1l = s1l + s2P
            s2l = s2l + s2P + s2I
            s3l = s3l + s2P + s2I + s2G
c            write(*,*) 'variables=',nc,MSG,MSI,s2G,s2I,s2P,s1l,s2l,s3l
         enddo
         Fst = s1l/s3l
         Fit = s2l/s3l
         Fis = (Fit-Fst)/(1-Fst)
         s1 = s1 + s1l
         s2 = s2 + s2l
         s3 = s3 + s3l
c         write(*,*) 'variables=',Fst,Fit,Fis,s1,s2,s3
      enddo
      Fsttot = s1/s3
      Fittot = s2/s3
      Fistot = (Fittot-Fsttot)/(1-Fsttot)
      
      end subroutine fstae
      

