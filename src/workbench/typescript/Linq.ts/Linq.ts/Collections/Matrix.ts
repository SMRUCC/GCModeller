class Matrix<T> extends IEnumerator<T[]> {

    public get rows(): number {
        return this.sequence.length;
    }

    public get columns(): number {
        return this.sequence[0].length;
    }

    /**
     * [m, n], m列n行
    */
    public constructor(m: number, n: number, fill: T = null) {
        super(Matrix.emptyMatrix<T>(m, n, fill));
    }

    private static emptyMatrix<T>(m: number, n: number, fill: T): T[][] {
        let matrix: T[][] = [];

        for (var i: number = 0; i < n; i++) {
            matrix.push(DataExtensions.Dim(m, fill));
        }

        return matrix;
    }

    /**
     * Get or set matrix element value
    */
    public M(i: number, j: number, val: T = null): T {
        if (isNullOrUndefined(val)) {
            // get
            return this.sequence[i][j];
        } else {
            this.sequence[i][j] = val;
        }
    }

    public column(i: number, set: T[] | IEnumerator<T> = null): T[] {
        if (isNullOrUndefined(set)) {
            // get
            let col: T[] = [];

            for (var j: number = 0; j < this.rows; j++) {
                col.push(this.sequence[j][i]);
            }

            return col;
        } else {
            let col: T[];

            if (Array.isArray(set)) {
                col = set;
            } else {
                col = set.ToArray(false);
            }

            for (var j: number = 0; j < this.rows; j++) {
                this.sequence[j][i] = col[j];
            }

            return null;
        }
    }

    public row(i: number, set: T[] | IEnumerator<T> = null): T[] {
        if (isNullOrUndefined(set)) {
            // get
            return this.sequence[i];
        } else {
            if (Array.isArray(set)) {
                this.sequence[i] = set;
            } else {
                this.sequence[i] = set.ToArray(false);
            }
        }
    }

    public toString(): string {
        return `[${this.rows}, ${this.columns}]`;
    }
}