import {ChangeDetectionStrategy, Component, input} from '@angular/core';
import {Matchup, Response} from "../app.component";
import {
    MatCell,
    MatCellDef,
    MatColumnDef,
    MatHeaderCell,
    MatHeaderCellDef,
    MatHeaderRow,
    MatHeaderRowDef,
    MatRow,
    MatRowDef,
    MatTable
} from "@angular/material/table";
import {CdkDrag, CdkDropList, moveItemInArray} from "@angular/cdk/drag-drop";
import {MatIcon} from "@angular/material/icon";
import {toObservable, toSignal} from "@angular/core/rxjs-interop";
import {concat, first, map, of, Subject, switchMap} from "rxjs";
import {HttpClient, HttpParams} from "@angular/common/http";

@Component({
    selector: 'matchup-table',
    imports: [
        MatTable,
        CdkDropList,
        MatHeaderCell,
        MatCell,
        MatIcon,
        MatHeaderRow,
        MatRow,
        CdkDrag,
        MatColumnDef,
        MatHeaderCellDef,
        MatCellDef,
        MatHeaderRowDef,
        MatRowDef
    ],
    templateUrl: './matchup-table.component.html',
    styleUrl: './matchup-table.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
})

export class MatchupTable {
    public inputRows = input.required<PlayerRow[], Response>({
        transform: this.mapResponse
    });

    public minGames = input.required<number>();
    public courtCount = input.required<number>();
    public readonly displayedColumns: string[] = [
        'position',
        'courtIndex',
        'playerIndex',
        'name'
    ];

    private readonly updatedDatasource$: Subject<PlayerRow[]> = new Subject<PlayerRow[]>();
    public readonly state = toSignal(
        concat(
            toObservable(this.inputRows).pipe(first()),
            this.updatedDatasource$.pipe(
                switchMap((datasource) => concat(
                    of(datasource),
                    this.getObs(datasource)
                ))
            )
        ), {initialValue: []}
    );

    public constructor(private http: HttpClient) {
    }

    private mapResponse(r: Response): PlayerRow[] {
        const rows: PlayerRow[] = [];
        for (const [courtIndex, matchupCollection] of Object.entries(r)) {
            for (const [index, name] of Object.entries(matchupCollection.players)) {
                rows.push({
                    courtIndex,
                    playerIndex: (parseInt(index) + 1).toString(),
                    name,
                    matchups: matchupCollection.matchups.filter(m =>
                        [m.pairing1.player1, m.pairing1.player2, m.pairing2.player1, m.pairing2.player2].includes(name)
                    )
                })
            }
        }
        return rows;
    }

    public drop(table: MatTable<PlayerRow>, movedName: string, currentIndex: number) {
        const dataSource = [...(table.dataSource as PlayerRow[])];
        const getNames = () => dataSource.map(r => r.name);
        const names = getNames();
        const previousIndex = names.findIndex(n => n === movedName);
        moveItemInArray(dataSource, previousIndex, currentIndex);
        this.updatedDatasource$.next(dataSource);
    }

    public trackByName(index: number, item: PlayerRow): string {
        return item.name;
    }

    private getObs(datasource: PlayerRow[]) {
        const params = new HttpParams({
            fromObject: {
                names: datasource.map(r => r.name),
                minGames: this.minGames(),
                courtCount: this.courtCount()
            }
        });

        return this.http.get<Response>(`http://localhost:5287/api/`, {params}).pipe(
            map(res => this.mapResponse(res)),
        );
    }
}

interface PlayerRow {
    courtIndex: string
    playerIndex: string
    name: string
    matchups: Matchup[]
}