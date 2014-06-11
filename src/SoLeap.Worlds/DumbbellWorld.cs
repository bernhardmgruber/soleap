using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override void SetupScene()
        {
            // ground
            var groundShape = new StaticPlaneShape(Vector3.UnitY, FloorHeight);
            Add(groundShape);
            CreateAndAddRigidBody(0.0f, Matrix.Identity, groundShape, "ground");

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

            CreateAndAddRigidBody(10f, Matrix.Translation(0, 0, FloorHeight + weightCylinderDiameter2 * 2), dumbbellshape, "Dumbbell");
        }
    }
}
