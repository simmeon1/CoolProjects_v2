import {ChangeDetectionStrategy, Component, signal} from '@angular/core';
import {HttpParams, httpResource} from "@angular/common/http";
import {MatFormField, MatInput, MatLabel} from "@angular/material/input";
import {CdkTextareaAutosize} from "@angular/cdk/text-field";
import {form, FormField, max, min, required} from "@angular/forms/signals";
import {MatCheckbox} from "@angular/material/checkbox";
import {MatButton} from "@angular/material/button";
import shuffle from "knuth-shuffle-seeded";
import {MatchupTable} from "./matchup-table/matchup-table.component";
import {MatTab, MatTabGroup} from "@angular/material/tabs";

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
        MatchupTable,
        MatTabGroup,
        MatTab
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
        return `https://thebadinbadminton-api.onrender.com/api/?${params.toString()}`;
    });
    public readonly form = form(signal<Form>({
            names: `Alfa
Bravo
Charlie
Delta
Echo
Foxtrot
Golf
Hotel
India
Juliett
Kilo
Lima
Mike`,
            minGames: 4,
            courtCount: 2,
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
    public readonly selectedTab = signal<number>(0);

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
        this.selectedTab.set(1);
    }
}

export type Response = Record<string, MatchupCollection>;

export interface MatchupCollection {
    players: Record<string, string>;
    matchups: Matchup[];
}

export interface Matchup {
    pairing1: Pairing;
    pairing2: Pairing;
}

export interface Pairing {
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