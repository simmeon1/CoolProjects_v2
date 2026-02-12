import {ChangeDetectionStrategy, Component, signal} from '@angular/core';
import {HttpParams, httpResource} from "@angular/common/http";
import {MatFormField, MatInput, MatLabel} from "@angular/material/input";
import {CdkTextareaAutosize} from "@angular/cdk/text-field";
import {form, FormField, max, min, required} from "@angular/forms/signals";
import {MatCheckbox} from "@angular/material/checkbox";
import {MatButton} from "@angular/material/button";
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
import shuffle from "knuth-shuffle-seeded";
import {CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray} from "@angular/cdk/drag-drop";
import {KeyValue, KeyValuePipe} from "@angular/common";
import {MatIcon} from "@angular/material/icon";

@Component({
    selector: 'app-root',
    imports: [
        MatFormField,
        MatLabel,
        CdkTextareaAutosize,
        MatInput,
        MatCheckbox,
        MatButton,
        FormField,
        CdkDropList,
        KeyValuePipe,
        CdkDrag,
        MatTable,
        MatHeaderCell,
        MatCell,
        MatIcon,
        MatHeaderRow,
        MatRow,
        MatColumnDef,
        MatHeaderCellDef,
        MatRowDef,
        MatHeaderRowDef,
        MatCellDef
    ],
    templateUrl: './app.component.html',
    styleUrl: './app.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class App {
    public readonly state = httpResource<Response>(() => {
        const p = this.fetchWithParams();
        const params = new HttpParams({
            fromObject: {
                names: p.names,
                minGames: p.minGames,
                courtCount: p.courtCount
            }
        });
        return `http://localhost:5287/api/?${params.toString()}`;
    });
    public readonly displayedColumns: string[] = ['position', 'name'];
    public readonly form = form(signal<Form>({
            names: `Alfa
Bravo
Charlie
Delta
Echo
Foxtrot
Golf`,
            minGames: 4,
            courtCount: 1,
            shuffle: false
        }), (schemaPath) => {
            required(schemaPath.names);
            required(schemaPath.minGames);
            min(schemaPath.minGames, 1);
            max(schemaPath.minGames, 10);
            required(schemaPath.courtCount);
            min(schemaPath.courtCount, 1);
            max(schemaPath.courtCount, 10);
        }
    );
    private readonly fetchWithParams = signal<Params>(this.getParamsFromForm());

    private getParamsFromForm(): Params {
        return {
            names: this.form.names().value().split('\n').map(n => n.trim()),
            minGames: this.form.minGames().value(),
            courtCount: this.form.courtCount().value()
        }
    }

    public onSubmit($event: SubmitEvent) {
        $event.preventDefault();
        const p = this.getParamsFromForm();
        if (this.form.shuffle().value()) {
            shuffle(p.names);
        }
        this.fetchWithParams.set(p);
    }

    public drop(event: CdkDragDrop<string>, table: MatTable<KeyValue<string, string>>) {
        const dataSource = table.dataSource as KeyValue<string, string>[];
        const previousIndex = dataSource.findIndex(d => d === event.item.data);
        moveItemInArray(dataSource, previousIndex, event.currentIndex);
        this.fetchWithParams.update((p): Params => ({
            names: dataSource.map(x => x.value),
            minGames: p.minGames,
            courtCount: p.courtCount,
        }))
    }
}

type Response = Record<string, MatchupCollection>;

interface MatchupCollection {
    players: Record<string, string>;
    matchups: Matchup[];
}

interface Matchup {
    pairing1: Pairing;
    pairing2: Pairing;
}

interface Pairing {
    player1: string;
    player2: string;
}

interface Params {
    names: string[];
    minGames: number;
    courtCount: number;
}

interface Form {
    names: string;
    minGames: number;
    courtCount: number;
    shuffle: boolean;
}