using System;

namespace TerraformingTerrain2d
{
    public struct NativeMarchingSquareConfigurationsPresenter : IDisposable
    {
        public NativeMarchingSquareMeshConfiguration Value_0 { get; set; }
        public NativeMarchingSquareMeshConfiguration Value_1 { get; set; }
        public NativeMarchingSquareMeshConfiguration Value_2 { get; set; }
        public NativeMarchingSquareMeshConfiguration Value_3 { get; set; }
        public NativeMarchingSquareMeshConfiguration Value_4 { get; set; }
        public NativeMarchingSquareMeshConfiguration Value_5 { get; set; }
        public NativeMarchingSquareMeshConfiguration Value_6 { get; set; }
        public NativeMarchingSquareMeshConfiguration Value_7 { get; set; }
        public NativeMarchingSquareMeshConfiguration Value_8 { get; set; }
        public NativeMarchingSquareMeshConfiguration Value_9 { get; set; }
        public NativeMarchingSquareMeshConfiguration Value_10 { get; set; }
        public NativeMarchingSquareMeshConfiguration Value_11 { get; set; }
        public NativeMarchingSquareMeshConfiguration Value_12 { get; set; }
        public NativeMarchingSquareMeshConfiguration Value_13 { get; set; }
        public NativeMarchingSquareMeshConfiguration Value_14 { get; set; }
        public NativeMarchingSquareMeshConfiguration Value_15 { get; set; }

        public NativeMarchingSquareMeshConfiguration this[int i] => i switch
        {
            0 => Value_0,
            1 => Value_1,
            2 => Value_2,
            3 => Value_3,
            4 => Value_4,
            5 => Value_5,
            6 => Value_6,
            7 => Value_7,
            8 => Value_8,
            9 => Value_9,
            10 => Value_10,
            11 => Value_11,
            12 => Value_12,
            13 => Value_13,
            14 => Value_14,
            15 => Value_15,
            _ => default
        };

        public void Dispose()
        {
            Value_0.Dispose();
            Value_1.Dispose();
            Value_2.Dispose();
            Value_3.Dispose();
            Value_4.Dispose();
            Value_5.Dispose();
            Value_6.Dispose();
            Value_7.Dispose();
            Value_8.Dispose();
            Value_9.Dispose();
            Value_10.Dispose();
            Value_11.Dispose();
            Value_12.Dispose();
            Value_13.Dispose();
            Value_14.Dispose();
            Value_15.Dispose();
        }
    }
}