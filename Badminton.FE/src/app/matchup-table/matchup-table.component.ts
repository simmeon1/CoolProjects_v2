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

    public drop(rows: PlayerRow[], movedName: string, currentIndex: number) {
        const names = rows.map(r => r.name);
        const previousIndex = names.findIndex(n => n === movedName);
        moveItemInArray(names, previousIndex, currentIndex);
        this.reorderedNames.set(names);
    }
}

interface PlayerRow {
    courtIndex: string
    playerIndex: string
    name: string
    matchups: Matchup[]
}