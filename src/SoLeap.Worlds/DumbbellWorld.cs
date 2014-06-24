using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SoLeap.Worlds
{
    public class DumbbellWorld : AbstractWorld
    {
        const float barLength = 200;

        private const float FloorHeight = 100f;
        private const float WeightCylinderDiameter = 150f;

        public DumbbellWorld()
            : base("Dumbbell", new Vector3(0, -500, 0))
        { }

        protected override void SetupScene()
        {
            // ground
            var groundShape = new StaticPlaneShape(Vector3.UnitY, FloorHeight);
            Add(groundShape);
            var body = CreateAndAddRigidBodyAndRenderable(0.0f, Matrix.Identity, groundShape, Colors.LightSlateGray, "ground");

            // dumbbell
            float barLength2 = barLength / 2.0f;
            float weightCylinderDiameter2 = WeightCylinderDiameter / 2.0f;

            var cylinderShape = new CylinderShapeX(20, weightCylinderDiameter2, weightCylinderDiameter2);
            var barShape = new CylinderShapeX(barLength2, 15, 15);
            var dumbbellshape = new CompoundShape();
            dumbbellshape.AddChildShape(Matrix.Identity, barShape);
            dumbbellshape.AddChildShape(Matrix.Translation(-barLength2, 0, 0), cylinderShape);
            dumbbellshape.AddChildShape(Matrix.Translation(+barLength2, 0, 0), cylinderShape);

            Add(cylinderShape);
            Add(barShape);
            Add(dumbbellshape);

            body = CreateAndAddRigidBodyAndRenderable(1f, Matrix.Translation(0, FloorHeight + weightCylinderDiameter2 * 2, 0), dumbbellshape, Colors.Gray, "Dumbbell");
            body.SetDamping(0.2f, 0.2f);
        }
    }
}
