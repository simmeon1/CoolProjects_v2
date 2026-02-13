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
import {CdkDrag, CdkDragHandle, CdkDropList, moveItemInArray} from "@angular/cdk/drag-drop";
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
        MatRowDef,
        CdkDragHandle
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

    private readonly updatedDatasource = signal<PlayerRow[] | undefined>(undefined);
    private httpResourceRef = httpResource<Response>(() => {
        const updatedDatasource = this.updatedDatasource();
        if (!updatedDatasource) {
            return undefined;
        }
        const params = new HttpParams({
            fromObject: {
                names: updatedDatasource.map(r => r.name),
                minGames: this.minGames(),
                courtCount: this.courtCount()
            }
        });
        return `http://localhost:5287/api/?${params.toString()}`;
    });

    public readonly state = computed((): PlayerRow[] => {
        const updatedDatasource = this.updatedDatasource();
        if (!updatedDatasource) {
            return this.inputRows();
        }
        return this.httpResourceRef.hasValue() ? this.mapResponse(this.httpResourceRef.value()) : updatedDatasource;
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
        const dataSource = [...(table.dataSource as PlayerRow[])];
        const getNames = () => dataSource.map(r => r.name);
        const names = getNames();
        const previousIndex = names.findIndex(n => n === movedName);
        moveItemInArray(dataSource, previousIndex, currentIndex);
        this.updatedDatasource.set(dataSource);
    }

    public trackByName(index: number, item: PlayerRow): string {
        return item.name;
    }
}

interface PlayerRow {
    courtIndex: string
    playerIndex: string
    name: string
    matchups: Matchup[]
}