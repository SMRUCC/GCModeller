namespace Levenshtein {

    export interface IScoreFunc {
        insert(c: string | number): number;
        delete(c: string | number): number;
        substitute(s: string | number, t: string | number): number;
    }

    const defaultScore: IScoreFunc = {
        insert: x => 1,
        delete: x => 1,
        substitute: function (s, t) {
            if (s == t) {
                return 0;
            } else {
                return 1;
            }
        }
    }

    export function DistanceMatrix(source: string, target: string, score: IScoreFunc = defaultScore): number[][] {
        let src: number[] = <number[]>Strings.ToCharArray(source, true);
        let tar: number[] = <number[]>Strings.ToCharArray(target, true);

        if (src.length == 0 && tar.length == 0) {
            return [[0]];
        }
        if (src.length == 0) {
            return [[$ts(tar).Sum(c => score.insert(c))]];
        } else if (tar.length == 0) {
            return [[$ts(src).Sum(c => score.delete(c))]];
        }

        let ns = src.length + 1;
        let nt = tar.length + 1;

        let d = new Matrix<number>(ns, nt, 0.0);

        d.column(0, Enumerable.Range(0, ns - 1));
        d.row(0, Enumerable.Range(0, nt - 1));

        for (var j: number = 1; j < nt; j++) {
            for (var i: number = 1; i < ns; i++) {
                d.M(i, j, Enumerable.Min(
                    d.M(i - 1, j) + score.delete(src[i - 1]),
                    d.M(i, j - 1) + score.insert(tar[j - 1]),
                    d.M(i - 1, j - 1) + score.substitute(src[i - 1], tar[j - 1])
                ));
            }
        }

        return d.ToArray(false);
    }

    export function ComputeDistance(source: string, target: string, score: IScoreFunc = defaultScore): number {
        let d: number[][] = DistanceMatrix(source, target, score);
        let distance: number = d[d.length - 1][d[0].length - 1];

        return distance;
    }
}