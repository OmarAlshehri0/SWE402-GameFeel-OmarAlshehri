# SWE402 Game Feel Assignment

## Student

- Name: Omar Alshehri
- Student ID: 202177130

## Project Information

- Unity version: 6000.3.2f1
- Before scene: `Assets/_Project/Scenes/Before.unity`
- After scene: `Assets/_Project/Scenes/After.unity`

## How to Play

Move the paddle using:

- Mouse
- A and D keys
- Left and Right arrow keys

The ball launches automatically when the game starts.

## Intended Feel

I wanted the game to feel more responsive, clear, and satisfying to play. I added visual and audio feedback so the player can understand what happens during the game.

## Improvements

| Improvement | When it happens | Implementation | Purpose |
|---|---|---|---|
| Brick impact effect | When a brick is destroyed | `Brick.cs` and brick feedback components | Shows an effect at the brick position. |
| Reactive animation | When the ball hits the paddle | `FeedbackManager` and paddle visual | Makes the paddle react to the collision. |
| Event-based audio | Paddle hit, brick destroyed, and ball lost | `GameEvents` and `FeedbackManager` | Gives different sounds to important events. |
| Score feedback | When the score increases | `ScoreText` and score feedback | Makes the score change easier to notice. |
| Ball trail and life feedback | During ball movement and life loss | Ball `Trail Renderer` and `LastLifeMode` | Makes the ball easier to follow and shows danger when lives are low. |

## Before and After

The `Before` scene is the original version of the game.

The `After` scene contains the improvements for game feel. The main brick-breaker gameplay is still the same.

## Playtest and Revision

I asked another person to try the Before and After versions.

They said that the After version felt better because the collisions had stronger visual and sound feedback. They also said that the life warning was a little strong.

After this feedback, I reduced the warning color intensity and adjusted some effect sizes. The result was easier to see without being too distracting.

## Demonstration Video

Video link:

`https://drive.google.com/file/d/1THlGORhUnM5vOcq_zJ-4SQSVwbSHSYEf/view?usp=sharing`

The video will show the Before scene, the After scene, the five improvements, one script or Inspector setting, and the revision based on playtest feedback.

## AI Use

I used AI to help me write some C# code, understand Unity errors, and fix a few mistakes.

I tested the code in Unity myself and changed the Inspector values and effects until they worked correctly.

## Assets and Credits

I downloaded some visual effects and assets from the Unity Asset Store, including:

- Cartoon FX Remaster Free
- Free 2D Mega Pack

The original starter project and its license information are included in the project files.
