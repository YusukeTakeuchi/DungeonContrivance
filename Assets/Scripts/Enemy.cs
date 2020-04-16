using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Floor;
using UnityEngine;
using Util.MatrixUtil;
using Util.RandomUtil;

public class Enemy : MonoBehaviour
{

    private struct Action
    {
        public Vector2Int? posTo;
        public Dir dir;
        //public Vector2Int? destination;
        public bool resetDestination;
        public bool endTurn;
    }

    private const int MOVING_DURATION = 8;

    private FloorData floor;

    public Vector2Int? posPrev;
    public Vector2Int posFrom;
    public Vector2Int? posTo;

    public Dir dir;

    private int movingCount = 0;

    

    public Vector2Int pos
    {
        get => this.posFrom;

        set
        {
            posTo = posFrom = value;
        }
    }

    public Vector2Int? destination;

    // Start is called before the first frame update
    void Start()
    {
        floor = Global.GetInstance().floor;

        posPrev = null;
        posFrom = GetRandomRoomPos();
        posTo = null;

        // TODO: make better
        dir = Dir.Down;

        SetTransformPos();
    }

    // Update is called once per frame
    private void Update()
    {
        SetTransformPos();
    }

    void FixedUpdate()
    {
        movingCount = (movingCount + 1) % MOVING_DURATION;
        if (movingCount == 0)
        {
            // finish and restart moving
            if (posTo is Vector2Int posToReal)
            {
                posPrev = posFrom;
                posFrom = posToReal;
            }
            DoAction();
        }
        else
        {
            // moving
        }
    }

    private void DoAction()
    {
        Action action;
        //var attr = floor.GetAttrAt(this.pos);
        if (this.destination is Vector2Int destReal)
        {
            action = GoForOrResetDestination(destReal);
            ApplyAction(action);
            if (action.endTurn)
            {
                return;
            }
        }

        Vector2Int? dest = GetNewDestination();
        if (dest is Vector2Int destReal2)
        {
            this.destination = dest;
            action = GoForOrResetDestination(destReal2);
        }
        else
        {
            action = GoForward();
        }
        ApplyAction(action);
        if (action.posTo == null)
        {
            action = new Action()
            {
                dir = Dir.Random(),
            };
            ApplyAction(action);
        }

    }

    private Action GoForOrResetDestination(Vector2Int dest)
    {
        var dir = Dir.FromDelta(dest - this.pos);
        if (dir == Dir.None)
        {
            // here is the destination
            return new Action()
            {
                resetDestination = true,
            };
        }
        var stepDir = SearchDirToStep(this.pos, new[] {
            dir,
            dir.TurnClockwise(),
            dir.TurnCounterclockwise(),
        });
        if (stepDir != null)
        {
            return new Action()
            {
                dir = stepDir,
                posTo = this.pos + stepDir,
                endTurn = true,
            };
        }
        else
        {
            return new Action()
            {
                resetDestination = true,
            };
        }
    }

    private Vector2Int? GetNewDestination()
    {
        var attr = floor.GetAttrAt(this.pos);
        if (attr.IsRoom())
        {
            var room = attr.room;
            if (room == null)
            {
                return null; // should not happen
            }
            return FindEntrance(room);
        }
        else
        {
            return null;
        }
    }

    // TODO: do not select the entrance which the enemy has just passed
    private Vector2Int? FindEntrance(Room room)
    {
        Dir[] dirs = new[] { Dir.Up, Dir.Left, Dir.Down, Dir.Right };
        var entrances = LoopWithinRect(room.rect)
            .SelectMany(pos =>
                dirs.Select(d => pos + d).Where(posD => floor.GetAttrAt(posD).IsPassage())
            );
        if (entrances.Count() == 0)
        {
            return null;
        }
        else
        {
            var entrancesExceptPrevPos = entrances.Where(ePos => ePos != this.posPrev);
            return (entrancesExceptPrevPos.Count() == 0 ? entrances : entrancesExceptPrevPos).TakeRandom();
        }
    }

    private IEnumerable<Vector2Int> LoopWithinRect(RectInt rect)
    {
        foreach (var elt in rect.allPositionsWithin)
        {
            yield return elt;
        }
    }

    /*
    private void StartStep()
    {
        var dir = NextStepDir(posFrom);
        if (dir == null)
        {
            posTo = posFrom;
            this.dir = Dir.Random();
        }
        else
        {
            posTo = posFrom + dir;
            this.dir = dir;
        }
    }
    */

    private Action GoForward()
    {
        int[] relDirCounts = { 0, 1, -1, 2, -2 };
        foreach (int count in relDirCounts)
        {
            var dirToCheck = this.dir.TurnClockwise(count);
            var posToCheck = this.pos + dirToCheck;
            var attr = floor.GetAttrAt(posToCheck);
            if (attr.CanEnter())
            {
                return new Action()
                {
                    dir = dirToCheck,
                    posTo = posToCheck,
                    endTurn = true,
                };
            }
        }
        return new Action();
    }

    private Dir SearchDirToStep(Vector2Int curPos, IEnumerable<Dir> dirs)
    {
        return dirs.FirstOrDefault(dir => floor.GetAttrAt(curPos + dir).CanEnter());
    }

    private Vector2Int GetRandomRoomPos()
    {
        var (attr, y, x) = floor.attrs.EachWithIndex()
            .Where<(FloorAttribute attr, int y, int x)>(
                t => t.attr.IsRoom()
            )
            .TakeRandom();
        return new Vector2Int(x, y);
    }

    private void ApplyAction(Action action)
    {
        if (action.posTo != null)
        {
            this.posTo = action.posTo;
        }
        if (action.dir != null)
        {
            this.dir = action.dir;
        }
        /*
        if (action.destination != null)
        {
            this.destination = action.destination;
        }
        */
        if (action.resetDestination)
        {
            this.destination = null;
        }
    }

    private void SetTransformPos()
    {
        var charaSize = gameObject.GetComponent<SpriteRenderer>().bounds.size;
        var position = gameObject.transform.position;

        if (this.posTo is Vector2Int posToReal)
        {
            var delta = posToReal - this.posFrom;
            position.x =
                this.posFrom.x * Background.TILE_SIZE +
                delta.x * movingCount * Background.TILE_SIZE / MOVING_DURATION;
            position.y = -(
                this.posFrom.y * Background.TILE_SIZE +
                delta.y * movingCount * Background.TILE_SIZE / MOVING_DURATION
            );
        }
        else
        {
            position.x = this.posFrom.x * Background.TILE_SIZE;
            position.y = -(this.posFrom.y * Background.TILE_SIZE);
        }

        gameObject.transform.position = position;
    }

}
