using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private Brick brickPrefab;
    [SerializeField] private int columns = 7;
    [SerializeField] private int rows = 4;

    [SerializeField] private Vector2 brickSize =
        new Vector2(2f, 0.7f);

    [SerializeField] private Vector2 spacing =
        new Vector2(0.15f, 0.15f);

    [SerializeField] private float topY = 4f;

    [Header("Colors")]
    [SerializeField] private Color[] rowColors;

    [Header("Infection Bricks")]
    [SerializeField, Range(0f, 1f)]
    private float infectionChance = 0.05f;

    [Header("Boss Bricks")]
    [SerializeField, Range(0f, 1f)]
    private float bossChance = 0.08f;

    [SerializeField] private int maximumBossBricks = 3;

    private int nextRowIndex;
    private int bossCount;

    private void Start()
    {
        Build();
    }

    public void Build()
    {
        float stepX =
            brickSize.x + spacing.x;

        float stepY =
            brickSize.y + spacing.y;

        float totalWidth =
            columns * stepX - spacing.x;

        float startX =
            -totalWidth * 0.5f
            + brickSize.x * 0.5f;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Vector3 position = new Vector3(
                    startX + column * stepX,
                    topY - row * stepY,
                    0f
                );

                CreateBrick(position, row, column);
            }
        }

        nextRowIndex = rows;
    }

    public void SpawnFallingRow()
    {
        float stepY =
            brickSize.y + spacing.y;

        Brick[] existingBricks =
            GetComponentsInChildren<Brick>();

        foreach (Brick brick in existingBricks)
        {
            if (brick == null)
                continue;

            Vector3 position =
                brick.transform.position;

            position.y -= stepY;
            brick.transform.position = position;
        }

        float stepX =
            brickSize.x + spacing.x;

        float totalWidth =
            columns * stepX - spacing.x;

        float startX =
            -totalWidth * 0.5f
            + brickSize.x * 0.5f;

        for (int column = 0; column < columns; column++)
        {
            Vector3 position = new Vector3(
                startX + column * stepX,
                topY,
                0f
            );

            CreateBrick(
                position,
                nextRowIndex,
                column
            );
        }

        nextRowIndex++;
    }

    private void CreateBrick(
        Vector3 position,
        int row,
        int column
    )
    {
        Brick brick =
            Instantiate(
                brickPrefab,
                position,
                Quaternion.identity,
                transform
            );

        brick.name =
            $"Brick_r{row}_c{column}";

        brick.transform.localScale =
            new Vector3(
                brickSize.x,
                brickSize.y,
                1f
            );

        if (rowColors != null &&
            rowColors.Length > 0 &&
            brick.TryGetComponent(
                out SpriteRenderer renderer))
        {
            Color color =
                rowColors[row % rowColors.Length];

            if (color.a <= 0.01f)
                color.a = 1f;

            renderer.color = color;
        }

        bool makeBoss =
            bossCount < maximumBossBricks &&
            Random.value <= bossChance;

        if (makeBoss)
        {
            brick.ConfigureBoss(true);
            bossCount++;
        }
        else
        {
            bool makeInfection =
                Random.value <= infectionChance;

            brick.ConfigureInfection(
                makeInfection
            );
        }
    }
}
