// export R# package module type define for javascript/typescript language
//
//    imports "Vina" from "molecular_kit";
//
// ref=molecular_kit.Vina@molecular_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace Vina {
   /**
     * @param exhaustiveness default value Is ``8``.
     * @param steps_per_run default value Is ``30``.
     * @param num_modes default value Is ``9``.
     * @param min_rmsd default value Is ``1.5``.
     * @param box_center default value Is ``null``.
     * @param box_half default value Is ``12``.
     * @param seed default value Is ``0``.
     * @param mmgbsa default value Is ``false``.
     * @param nwat default value Is ``0``.
     * @param mmgbsa_top default value Is ``3``.
     * @param env default value Is ``null``.
   */
   function dock_options(exhaustiveness?: object, steps_per_run?: object, num_modes?: object, min_rmsd?: number, box_center?: any, box_half?: number, seed?: object, mmgbsa?: boolean, nwat?: object, mmgbsa_top?: object, env?: object): object;
   /**
     * @param add_hydrogens default value Is ``false``.
     * @param target_charge default value Is ``0``.
     * @param seed default value Is ``20240101``.
     * @param env default value Is ``null``.
   */
   function ligand_from_smiles(smiles: string, add_hydrogens?: boolean, target_charge?: number, seed?: object, env?: object): object;
   /**
     * @param ligand_chain default value Is ``null``.
     * @param ligand_resname default value Is ``null``.
     * @param nwat default value Is ``0``.
     * @param env default value Is ``null``.
   */
   function mmgbsa_complex(file: string, ligand_chain?: string, ligand_resname?: string, nwat?: object, env?: object): object;
   /**
     * @param nwat default value Is ``0``.
     * @param env default value Is ``null``.
   */
   function mmgbsa_score(receptor: object, ligand: object, nwat?: object, env?: object): object;
   /**
     * @param rank default value Is ``1``.
     * @param env default value Is ``null``.
   */
   function pose_atoms(result: object, rank?: object, env?: object): object;
   module read {
      /**
        * @param target_charge default value Is ``0``.
        * @param env default value Is ``null``.
      */
      function ligand(file: string, target_charge?: number, env?: object): object;
      /**
        * @param env default value Is ``null``.
      */
      function receptor(file: string, env?: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function vina_atom_types(env?: object): object;
   /**
   */
   function vina_atoms(mol: object): object;
   /**
     * @param options default value Is ``null``.
     * @param box_center default value Is ``null``.
     * @param box_half default value Is ``12``.
     * @param exhaustiveness default value Is ``8``.
     * @param steps_per_run default value Is ``30``.
     * @param num_modes default value Is ``9``.
     * @param min_rmsd default value Is ``1.5``.
     * @param seed default value Is ``0``.
     * @param mmgbsa default value Is ``false``.
     * @param nwat default value Is ``0``.
     * @param mmgbsa_top default value Is ``3``.
     * @param env default value Is ``null``.
   */
   function vina_dock(receptor: object, ligand: object, options?: any, box_center?: any, box_half?: number, exhaustiveness?: object, steps_per_run?: object, num_modes?: object, min_rmsd?: number, seed?: object, mmgbsa?: boolean, nwat?: object, mmgbsa_top?: object, env?: object): object;
   /**
     * @param options default value Is ``null``.
     * @param box_center default value Is ``null``.
     * @param box_half default value Is ``12``.
     * @param exhaustiveness default value Is ``8``.
     * @param steps_per_run default value Is ``30``.
     * @param num_modes default value Is ``9``.
     * @param min_rmsd default value Is ``1.5``.
     * @param seed default value Is ``0``.
     * @param mmgbsa default value Is ``false``.
     * @param nwat default value Is ``0``.
     * @param mmgbsa_top default value Is ``3``.
     * @param target_charge default value Is ``0``.
     * @param env default value Is ``null``.
   */
   function vina_dock_files(receptor: string, ligand: string, options?: any, box_center?: any, box_half?: number, exhaustiveness?: object, steps_per_run?: object, num_modes?: object, min_rmsd?: number, seed?: object, mmgbsa?: boolean, nwat?: object, mmgbsa_top?: object, target_charge?: number, env?: object): object;
   /**
     * @param rotatable default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function vina_score(receptor: object, ligand: object, rotatable?: boolean, env?: object): number;
   /**
     * @param receptor_atoms default value Is ``-1``.
     * @param mode default value Is ``'ligand'``.
     * @param options default value Is ``null``.
     * @param pretty default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function write_dock_report(result: object, file: string, receptor_atoms?: object, mode?: string, options?: any, pretty?: boolean, env?: object): string;
   /**
     * @param rank default value Is ``1``.
     * @param res_name default value Is ``'LIG'``.
     * @param chain default value Is ``'L'``.
     * @param title default value Is ``null``.
     * @param ligand default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function write_pose_pdb(result: object, file: string, rank?: object, res_name?: string, chain?: string, title?: string, ligand?: any, env?: object): string;
}
