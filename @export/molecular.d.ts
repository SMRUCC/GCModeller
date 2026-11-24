// export R# package module type define for javascript/typescript language
//
//    imports "molecular" from "molecular_kit";
//
// ref=molecular_kit.molecular_tools@molecular_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace molecular {
   /**
     * @param env default value Is ``null``.
   */
   function add_hydrogens(mol: object, env?: object): object;
   module as {
      /**
        * @param res_name default value Is ``'UNL'``.
        * @param chain default value Is ``'A'``.
        * @param res_seq default value Is ``1``.
        * @param hetatm default value Is ``true``.
        * @param include_h default value Is ``true``.
        * @param ensure_3d default value Is ``true``.
        * @param title default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function pdb(mol: object, res_name?: string, chain?: string, res_seq?: object, hetatm?: boolean, include_h?: boolean, ensure_3d?: boolean, title?: string, env?: object): object;
   }
   /**
     * @param bg default value Is ``'white'``.
     * @param bond_color default value Is ``'#222222'``.
     * @param bond_width default value Is ``2``.
     * @param bond_separation default value Is ``4``.
     * @param font default value Is ``'Arial'``.
     * @param atom_font_size default value Is ``16``.
     * @param title_font_size default value Is ``15``.
     * @param padding default value Is ``28``.
     * @param show_title default value Is ``true``.
     * @param title_color default value Is ``'#808080'``.
     * @param hide_carbon default value Is ``true``.
     * @param ball_scale default value Is ``0.5``.
     * @param spacefill_scale default value Is ``1``.
     * @param bond_radius default value Is ``0.13``.
     * @param auto_orient default value Is ``true``.
     * @param angle_x default value Is ``12``.
     * @param angle_y default value Is ``-8``.
     * @param angle_z default value Is ``0``.
     * @param fov default value Is ``320``.
     * @param sphere_stacks default value Is ``10``.
     * @param sphere_slices default value Is ``16``.
     * @param env default value Is ``null``.
   */
   function chem_theme(bg?: any, bond_color?: any, bond_width?: number, bond_separation?: number, font?: string, atom_font_size?: number, title_font_size?: number, padding?: object, show_title?: boolean, title_color?: any, hide_carbon?: boolean, ball_scale?: number, spacefill_scale?: number, bond_radius?: number, auto_orient?: boolean, angle_x?: number, angle_y?: number, angle_z?: number, fov?: number, sphere_stacks?: object, sphere_slices?: object, env?: object): object;
   /**
     * @param iterations default value Is ``600``.
     * @param add_hydrogens default value Is ``true``.
     * @param respect_stereo default value Is ``true``.
     * @param seed default value Is ``20240101``.
     * @param env default value Is ``null``.
   */
   function embed3d(mol: object, iterations?: object, add_hydrogens?: boolean, respect_stereo?: boolean, seed?: object, env?: object): object;
   /**
     * @param bond_length default value Is ``1``.
     * @param iterations default value Is ``300``.
     * @param ideal_angle default value Is ``120``.
     * @param min_separation default value Is ``0.85``.
     * @param seed default value Is ``12345``.
     * @param env default value Is ``null``.
   */
   function layout2d(mol: object, bond_length?: number, iterations?: object, ideal_angle?: number, min_separation?: number, seed?: object, env?: object): object;
   /**
   */
   function molecule_atoms(mol: object): object;
   /**
   */
   function molecule_bonds(mol: object): object;
   /**
   */
   function molecule_components(mol: object): any;
   /**
   */
   function molecule_coordinates(mol: object): object;
   /**
   */
   function molecule_format(mol: object): string;
   /**
   */
   function molecule_formula(mol: object): string;
   /**
   */
   function molecule_rings(mol: object): any;
   /**
   */
   function molecule_summary(mol: object): object;
   /**
   */
   function molecule_warnings(mol: object): string;
   /**
   */
   function molecule_weight(mol: object): number;
   /**
     * @param env default value Is ``null``.
   */
   function parse_inchi(inchi: string, env?: object): object;
   /**
     * @param format default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function parse_molecule(text: string, format?: string, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function parse_molecules(text: string, env?: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function parse_smiles(smiles: string, env?: object): object;
   module read {
      /**
        * @param format default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function molecule(file: any, format?: string, env?: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function remove_hydrogens(mol: object, env?: object): object;
   /**
     * @param dim default value Is ``2``.
     * @param mode default value Is ``'ball_and_stick'``.
     * @param size default value Is ``'1200,1000'``.
     * @param theme default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function save_png(mol: object, file: string, dim?: object, mode?: string, size?: any, theme?: any, env?: object): boolean;
   /**
     * @param dim default value Is ``2``.
     * @param mode default value Is ``'ball_and_stick'``.
     * @param size default value Is ``'1200,1000'``.
     * @param theme default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function save_svg(mol: object, file: string, dim?: object, mode?: string, size?: any, theme?: any, env?: object): boolean;
   module write {
      /**
        * @param res_name default value Is ``'UNL'``.
        * @param chain default value Is ``'A'``.
        * @param res_seq default value Is ``1``.
        * @param hetatm default value Is ``true``.
        * @param include_h default value Is ``true``.
        * @param ensure_3d default value Is ``true``.
        * @param title default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function pdb(mol: object, file: string, res_name?: string, chain?: string, res_seq?: object, hetatm?: boolean, include_h?: boolean, ensure_3d?: boolean, title?: string, env?: object): string;
   }
   /**
     * @param env default value Is ``null``.
   */
   function write_smiles(mol: object, env?: object): string;
}
