import {ChangeDetectionStrategy, Component, signal} from '@angular/core';
import {HttpParams, httpResource} from "@angular/common/http";
import {MatFormField, MatInput, MatLabel} from "@angular/material/input";
import {CdkTextareaAutosize} from "@angular/cdk/text-field";
import {form, FormField, max, min, required} from "@angular/forms/signals";
import {MatCheckbox} from "@angular/material/checkbox";
import {MatButton} from "@angular/material/button";
import shuffle from "knuth-shuffle-seeded";
import {JsonPipe} from "@angular/common";

@Component({
    selector: 'app-root',
    imports: [MatFormField, MatLabel, CdkTextareaAutosize, MatInput, MatCheckbox, MatButton, JsonPipe, FormField],
    templateUrl: './app.component.html',
    styleUrl: './app.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class App {
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
    private readonly fetchUrl = signal<string>(this.getUrl());
    public readonly state = httpResource<MatchupMap>(() => this.fetchUrl());

    public onSubmit($event: SubmitEvent) {
        $event.preventDefault();
        this.fetchUrl.set(this.getUrl());
    }

    private getUrl() {
        const names = this.form.names().value().split('\n').map(n => n.trim());
        if (this.form.shuffle().value()) {
            shuffle(names);
        }
        const params = new HttpParams({
            fromObject: {
                names,
                minGames: this.form.minGames().value(),
                courtCount: this.form.courtCount().value()
            }
        });
        return `http://localhost:5287/api/?${params.toString()}`;
    }
}

type MatchupMap = Record<string, Matchup>;

interface Matchup {
    pairing1: Pairing;
    pairing2: Pairing;
}

interface Pairing {
    player1: string;
    player2: string;
}

interface Form {
    names: string;
    minGames: number;
    courtCount: number;
    shuffle: boolean;
}