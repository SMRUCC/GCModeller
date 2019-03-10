***********************************************************************
* MCMC inference on Potts-Dirichlet (spatial) + Pritchard (genotype) model
*   
*     Routines used =   sub.f
*    
***********************************************************************
        
      subroutine mcmcgenecl(s,z,nall,
     &     filec,filepsi,filefis,fileloglik, 
     &     nchain,burnin,stepw,nindiv,nloc,nloc2,nallmax,
     &     nclass,fis,fistemp,alpha,beta,
     &     c,ctemp,f,ftemp,psi,psimax,
     &     matngh,nghup,ngh,numpsi,tabcst,
     &     priorpsi,varpsi,varfis)

      implicit none 

*     input data
      integer nclass,nindiv,nloc,nloc2,numpsi,
     &     nall,nallmax,z,nghup,ngh,matngh, 
     &     nchain,stepw,burnin
      double precision psimax,s,tabcst,alpha,beta,priorpsi
      integer c,varpsi,varfis
      double precision f,fis,fistemp,psi
*     dimensions
      dimension priorpsi(numpsi)


*     local variables
      integer ctemp,iloc,iindiv,ichain,iclass,iall
      double precision ftemp,loglikelihood
      character*200 filec,filepsi,filefis,fileloglik
 
*     dimensions
      dimension s(nindiv,2),z(nindiv,nloc2),
     &     c(nindiv),ctemp(nindiv),
     &     f(nclass,nloc,nallmax),fis(nclass),
     &     nall(nloc),fistemp(nclass),
     &     ftemp(nclass,nloc,nallmax),tabcst(numpsi), 
     &     matngh(nindiv,nindiv), 
     &     nghup(nindiv),ngh(nindiv)

    


      write(6,*) '          **************************************'
      write(6,*) '          ***    Starting MCMC inference     ***'
      write(6,*) '          **************************************'

*     init RNG
      call rndstart()
 
*     output files
      open(10,file=filec)
      open(11,file=filepsi)
      open(12,file=filefis)
      open(13,file=fileloglik)


************************
* MCMC updates 
************************

      do ichain=1,nchain

              if(mod(ichain,1000) .eq. 0) then
                  write(*,*) ichain
              endif

     
              if(mod(ichain,stepw) .eq. 0) then

                  write(13,*) loglikelihood(z,f,c,fis,nclass,nall,
     &                             nallmax,nindiv,nloc,nloc2)

                  if(ichain.gt.burnin) then  
 1000                   format (2(1x,e15.8,1x))

                        do iindiv=1,nindiv
                              write(10,*) c(iindiv)
                        enddo

                        write(11,*) psi

                        do iclass= 1,nclass
                              write(12,*) fis(iclass)
                        enddo
                  endif

             endif

  

c$$$c$$$c$$$        write(6,*) 'true f'
c$$$c$$$c$$$c$$$    write(6,*) 'update f'
             call rpostf(c,z,fis,f,ftemp,nclass,nindiv,nloc,nloc2,nall,
     &               nallmax)


c$$$         write(6,*) 'true c'
c$$$         write(6,*) 'update c'
              
             call rpostc(c,ctemp,z,nindiv,nloc,nloc2,nall,nallmax,
     &              nclass,f,matngh,psi,fis,nghup)


c$$$c$$$c$$$         write(6,*) 'true psi'
c$$$c$$$c$$$         write(6,*) 'update psi'
             if(varpsi .eq. 1) then              
                  call rpostpsi(nindiv,c,psi,tabcst,psimax,
     &                        numpsi,matngh,ngh,priorpsi)
             endif

c$$$c$$$         write(6,*) 'true fis'
c$$$c$$$         write(6,*) 'update fis'    
             if(varfis .eq. 1) then
                 call rpostfis(c,z,fis,fistemp,f,nclass,nindiv,
     &                      nloc,nloc2,nall,nallmax,alpha,beta)
             endif
      enddo
      call rndend()   


      close(10)
      close(11)
      close(12)
      close(13)
   
 
       
      write(6,*) '          ***************************************'
      write(6,*) '          ***    End of MCMC computations     ***'
      write(6,*) '          ***************************************'
   
       end


 
         

      

