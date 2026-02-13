import {ChangeDetectionStrategy, Component, computed, input, signal} from '@angular/core';
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
import {HttpParams, httpResource} from "@angular/common/http";
import {toSignal} from "@angular/core/rxjs-interop";
import {of} from "rxjs";

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

    public constructor() {
        let x = toSignal(of(1));
    }

    private readonly reorderedNames = signal<string[]>([]);
    private httpResourceRef = httpResource<Response>(() => {
        const names = this.reorderedNames();
        if (names.length === 0) return undefined;
        const params = new HttpParams({
            fromObject: {
                names,
                minGames: this.minGames(),
                courtCount: this.courtCount()
            }
        });
        return `http://localhost:5287/api/?${params.toString()}`;
    });

    public readonly state = computed((): PlayerRow[] => {
        const resourceValue = this.httpResourceRef.value();
        if (resourceValue) {
            return this.mapResponse(resourceValue);
        }
        return this.inputRows();
    });

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
        const dataSource = table.dataSource as PlayerRow[];
        const getNames = () => dataSource.map(r => r.name);
        const names = getNames();
        const previousIndex = names.findIndex(n => n === movedName);
        moveItemInArray(dataSource, previousIndex, currentIndex);
        // this.reorderedNames.set(getNames());
        table.renderRows();
    }
}

interface PlayerRow {
    courtIndex: string
    playerIndex: string
    name: string
    matchups: Matchup[]
}