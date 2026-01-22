using UnityEngine;

namespace SnakeVR
{
    public enum BoundaryType
    {
        Walls,          // Solid walls that cause game over
        Wraparound,     // Snake wraps to opposite side
        None            // No boundaries (dangerous!)
    }

    public class GridManager : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private Vector3 gridSize = new Vector3(5f, 3f, 5f);
        [SerializeField] private float gridStep = 0.3f;
        [SerializeField] private BoundaryType boundaryType = BoundaryType.Walls;

        [Header("Visual Settings")]
        [SerializeField] private bool showGrid = true;
        [SerializeField] private bool showBoundaries = true;
        [SerializeField] private Color gridColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        [SerializeField] private Color boundaryColor = new Color(1f, 0f, 0f, 0.5f);

        [Header("Wall Settings")]
        [SerializeField] private GameObject wallPrefab;
        [Tooltip("Assign TronGridWall material from Assets/SnakeVR/Materials/")]
        [SerializeField] private Material wallMaterial;

        private GameObject wallsContainer;

        private void Start()
        {
            if (boundaryType == BoundaryType.Walls)
            {
                CreateWalls();
            }
        }

        private void CreateWalls()
        {
            // Create container
            wallsContainer = new GameObject("Walls");
            wallsContainer.transform.SetParent(transform);
            wallsContainer.transform.localPosition = Vector3.zero;

            float halfX = gridSize.x / 2f;
            float halfY = gridSize.y / 2f;
            float halfZ = gridSize.z / 2f;
            float wallThickness = 0.5f; // Augmenté de 0.1 à 0.5 pour être visible!

            // Floor - Positionné à Y=0 au lieu de -halfY
            CreateWall(
                new Vector3(0, 0, 0),
                new Vector3(gridSize.x, wallThickness, gridSize.z)
            );

            // Ceiling - Plus haut pour laisser de l'espace
            CreateWall(
                new Vector3(0, gridSize.y, 0),
                new Vector3(gridSize.x, wallThickness, gridSize.z)
            );

            // Left wall
            CreateWall(
                new Vector3(-halfX - wallThickness / 2, gridSize.y / 2, 0),
                new Vector3(wallThickness, gridSize.y, gridSize.z)
            );

            // Right wall
            CreateWall(
                new Vector3(halfX + wallThickness / 2, gridSize.y / 2, 0),
                new Vector3(wallThickness, gridSize.y, gridSize.z)
            );

            // Front wall
            CreateWall(
                new Vector3(0, gridSize.y / 2, halfZ + wallThickness / 2),
                new Vector3(gridSize.x, gridSize.y, wallThickness)
            );

            // Back wall
            CreateWall(
                new Vector3(0, gridSize.y / 2, -halfZ - wallThickness / 2),
                new Vector3(gridSize.x, gridSize.y, wallThickness)
            );
        }

        private void CreateWall(Vector3 position, Vector3 size)
        {
            GameObject wall;

            if (wallPrefab != null)
            {
                wall = Instantiate(wallPrefab, wallsContainer.transform);
                wall.transform.localPosition = position;
                wall.transform.localScale = size;
            }
            else
            {
                wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.transform.SetParent(wallsContainer.transform);
                wall.transform.localPosition = position;
                wall.transform.localScale = size;

                // Apply material
                Renderer renderer = wall.GetComponent<Renderer>();
                if (renderer != null)
                {
                    if (wallMaterial != null)
                    {
                        renderer.material = wallMaterial;
                    }
                    else
                    {
                        // Try URP/Lit first, fallback to Universal Render Pipeline/Lit, then Standard
                        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                        if (shader == null)
                            shader = Shader.Find("URP/Lit");
                        if (shader == null)
                            shader = Shader.Find("Standard");
                        
                        Material mat = new Material(shader);
                        mat.color = new Color(0.3f, 0.3f, 0.3f, 1f); // Gris foncé opaque
                        renderer.material = mat;
                    }
                }
            }

            wall.tag = "Wall";
            wall.name = "Wall";

            // Ensure collider is set as trigger
            Collider col = wall.GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        public Vector3 WrapPosition(Vector3 position)
        {
            if (boundaryType != BoundaryType.Wraparound)
                return position;

            Vector3 wrappedPos = position;
            Vector3 halfSize = gridSize / 2f;

            // Wrap X
            if (position.x > halfSize.x)
                wrappedPos.x = -halfSize.x;
            else if (position.x < -halfSize.x)
                wrappedPos.x = halfSize.x;

            // Wrap Y
            if (position.y > halfSize.y)
                wrappedPos.y = -halfSize.y;
            else if (position.y < -halfSize.y)
                wrappedPos.y = halfSize.y;

            // Wrap Z
            if (position.z > halfSize.z)
                wrappedPos.z = -halfSize.z;
            else if (position.z < -halfSize.z)
                wrappedPos.z = halfSize.z;

            return wrappedPos;
        }

        public bool IsOutOfBounds(Vector3 position)
        {
            Vector3 halfSize = gridSize / 2f;

            return Mathf.Abs(position.x) > halfSize.x ||
                   Mathf.Abs(position.y) > halfSize.y ||
                   Mathf.Abs(position.z) > halfSize.z;
        }

        public Vector3 GetRandomPositionInGrid()
        {
            float x = Mathf.Round(Random.Range(-gridSize.x / 2, gridSize.x / 2) / gridStep) * gridStep;
            float y = Mathf.Round(Random.Range(-gridSize.y / 2, gridSize.y / 2) / gridStep) * gridStep;
            float z = Mathf.Round(Random.Range(-gridSize.z / 2, gridSize.z / 2) / gridStep) * gridStep;

            return transform.position + new Vector3(x, y, z);
        }

        public Vector3 SnapToGrid(Vector3 position)
        {
            Vector3 localPos = position - transform.position;
            localPos.x = Mathf.Round(localPos.x / gridStep) * gridStep;
            localPos.y = Mathf.Round(localPos.y / gridStep) * gridStep;
            localPos.z = Mathf.Round(localPos.z / gridStep) * gridStep;
            return transform.position + localPos;
        }

        private void OnDrawGizmos()
        {
            // Draw grid
            if (showGrid)
            {
                Gizmos.color = gridColor;
                DrawGridLines();
            }

            // Draw boundaries
            if (showBoundaries)
            {
                Gizmos.color = boundaryColor;
                Gizmos.DrawWireCube(transform.position, gridSize);
            }
        }

        private void DrawGridLines()
        {
            Vector3 halfSize = gridSize / 2f;

            // X-Z plane (horizontal grid)
            for (float x = -halfSize.x; x <= halfSize.x; x += gridStep)
            {
                Vector3 start = transform.position + new Vector3(x, 0, -halfSize.z);
                Vector3 end = transform.position + new Vector3(x, 0, halfSize.z);
                Gizmos.DrawLine(start, end);
            }

            for (float z = -halfSize.z; z <= halfSize.z; z += gridStep)
            {
                Vector3 start = transform.position + new Vector3(-halfSize.x, 0, z);
                Vector3 end = transform.position + new Vector3(halfSize.x, 0, z);
                Gizmos.DrawLine(start, end);
            }
        }

        public Vector3 GetGridSize()
        {
            return gridSize;
        }

        public float GetGridStep()
        {
            return gridStep;
        }

        public BoundaryType GetBoundaryType()
        {
            return boundaryType;
        }
    }
}
