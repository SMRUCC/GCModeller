****************************************************
*    Simulation according to Potts-Dirichlet model   
*
*****************************************************


      subroutine simpotts(nindiv,
     &                    nclass,
     &                    psi,
     &                    filec,
     &                    fileng,
     &                    nchain,
     &                    burnin,
     &                    stepw,
     &                    c, 
     &                    matngh,
     &                    ngh,
     &                    probcond,
     &                    tempmul)


      implicit none
      integer nclass,nindiv,c,matngh,ngh,tempmul,nchain,stepw,
     &         energy, energyc, burnin
      double precision psi, probcond
      character*200  filec, fileng

      dimension  matngh(nindiv,nindiv), probcond(nclass),
     &           c(nindiv),ngh(nindiv),tempmul(nclass)


*Temporary variables
      integer iclass,iindiv, ichain
 

      write(6,*)'******************************************************'
      write(6,*)'* Starting simulation from Potts-Dirichlet model...   '
      write(6,*)'******************************************************'

*     init RNG
      call rndstart()


*  Output files 
    
      open(9,file=filec)
      open(10,file=fileng)

*****************************************************
* MCMC updates by using Gibbs sampler 
*****************************************************

      do ichain=1,nchain

c          if(mod(ichain,1000) .eq. 0) then
              write(*,*)  ichain 
c          endif

           if(ichain.gt.burnin) then  
              if(mod(ichain,stepw) .eq. 0) then 
 1000             format (2(1x,e15.8,1x))
                  do iindiv=1,nindiv
                      write(9,*) c(iindiv)
                  enddo        
*Calculation of system energy in the configuration c 
                  energyc = energy(nindiv,c,matngh,ngh)
                  write(10,*) energyc

              endif
          endif

          call gibbsup(nindiv,nclass,c,matngh,ngh,probcond,psi,tempmul)
           
      enddo
      call rndend()  

      close(9)  
      close(10)

      write(6,*) ' ****************************************************'
      write(6,*) ' ***      populations spatial structure           ***'
      write(6,*) ' ***                        =                     ***'
      write(6,*) ' ***      Potts-Dirichlet configuration           ***'
      write(6,*) ' ****************************************************'
    
      write(6,*)'******************************************************'
      write(6,*)'* End of simulation from Potts-Dirichlet model...   '
      write(6,*)'******************************************************'

      end


     
