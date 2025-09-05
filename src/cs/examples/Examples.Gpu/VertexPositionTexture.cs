// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace Gpu;

public record struct VertexPositionTexture
{
    public Vector3 Position;
    public Vector2 TextureCoordinates;

    public static int SizeOf => Unsafe.SizeOf<VertexPositionTexture>();

    public static void RectangleNonIndexed(Span<VertexPositionTexture> data)
    {
        /*
         * NOTE: Model vertices of a rectangle using standard cartesian coordinate system. (Z=0)
         *      +X is to the right, -X to the left
         *      +Y is towards the sky (up), -Y is towards the ground (down)
         *  (-1,-1)              (1,-1)
         *  +--------------------+
         *  |                    |
         *  |                    |
         *  |                    |
         *  |                    |
         *  |                    |
         *  |                    |
         *  |                    |
         *  +--------------------+
         *  (-1,-1)             (1,-1)
         *
        * NOTE: Each rectangle is 4 vertices.
        * NOTE: Preferred counter-clockwise vertices.
        *      Triangle 1: 0 -> 1 -> 2
        *      Triangle 2: 0 -> 2 -> 3
        * 3 ----------- 2
        * |           / |
        * |         /   |
        * |       /     |
        * |     /       |
        * |   /         |
        * | /           |
        * 0 ----------- 1
        */

        const float positionLeftX = -1.0f;
        const float positionRightX = 1.0f;
        const float positionBottomY = -1.0f;
        const float positionTopY = 1.0f;

        var positionBottomLeft = new Vector2(positionLeftX, positionBottomY);
        var positionBottomRight = new Vector2(positionRightX, positionBottomY);
        var positionTopRight = new Vector2(positionRightX, positionTopY);
        var positionTopLeft = new Vector2(positionLeftX, positionTopY);

        /*
         * NOTE: Texture coordinates range from [0, 1] using a 2D image.
         *      +U is to the right, -U to the left
         *      +V is down to the bottom, -V is up towards the top
         *  (0,0)               (1,0)
         *  +--------------------+
         *  |                    |
         *  |                    |
         *  |                    |
         *  |                    |
         *  |                    |
         *  |                    |
         *  |                    |
         *  +--------------------+
         *  (0,1)               (1,1)
         */

        const float textureCoordinateLeftU = 0.0f;
        const float textureCoordinateRightU = 1.0f;
        const float textureCoordinateBottomV = 1.0f;
        const float textureCoordinatesTopV = 0.0f;

        var textureCoordinatesBottomLeft = new Vector2(textureCoordinateLeftU, textureCoordinateBottomV);
        var textureCoordinatesBottomRight = new Vector2(textureCoordinateRightU, textureCoordinateBottomV);
        var textureCoordinatesTopRight = new Vector2(textureCoordinateRightU, textureCoordinatesTopV);
        var textureCoordinatesTopLeft = new Vector2(textureCoordinateLeftU, textureCoordinateBottomV);

        data[0].Position = new Vector3(positionBottomLeft, 0);
        data[0].TextureCoordinates = textureCoordinatesTopLeft;

        data[1].Position = new Vector3(positionBottomRight, 0);
        data[1].TextureCoordinates = textureCoordinatesBottomRight;

        data[2].Position = new Vector3(positionTopRight, 0);
        data[2].TextureCoordinates = textureCoordinatesTopRight;

        data[3].Position = new Vector3(positionBottomLeft, 0);
        data[3].TextureCoordinates = textureCoordinatesBottomLeft;

        data[4].Position = new Vector3(positionTopRight, 0);
        data[4].TextureCoordinates = textureCoordinatesTopRight;

        data[5].Position = new Vector3(positionTopLeft, 0);
        data[5].TextureCoordinates = textureCoordinatesTopLeft;
    }
}
