namespace VPS.SWplugin.ICT.Parts
{
    class MaterialSelection
    {
        // Auto-implemented properties
        public string PartName { get; set; }
        public string SWMaterial { get; set; }
        public int Material_ID { get; set; }
        public string MaterialName { get; set; }


        /// <summary>
        /// constructor with no arguments
        /// </summary>
        public MaterialSelection()
        {

        }
        /// <summary>
        /// Constructor with all 4 arguments
        /// </summary>
        /// <param name="part"></param>
        /// <param name="swMaterial"></param>
        /// <param name="materialID"></param>
        /// <param name="material"></param>
        public MaterialSelection(string part, string swMaterial, int materialID, string material)
        {
            PartName = part;
            SWMaterial = swMaterial;
            Material_ID = materialID;
            MaterialName = material;
        }
    }
}
