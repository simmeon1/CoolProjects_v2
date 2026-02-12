import {ChangeDetectionStrategy, Component, signal} from '@angular/core';
import {HttpParams, httpResource} from "@angular/common/http";
import {MatFormField, MatInput, MatLabel} from "@angular/material/input";
import {CdkTextareaAutosize} from "@angular/cdk/text-field";
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {MatCheckbox} from "@angular/material/checkbox";
import {MatButton} from "@angular/material/button";
import shuffle from "knuth-shuffle-seeded";
import {JsonPipe} from "@angular/common";

@Component({
    selector: 'app-root',
    imports: [MatFormField, MatLabel, CdkTextareaAutosize, MatInput, ReactiveFormsModule, MatCheckbox, MatButton, JsonPipe],
    templateUrl: './app.component.html',
    styleUrl: './app.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class App {
    public readonly form: FormGroup<Form>
    private readonly fetchUrl = signal<string | undefined>(undefined);
    public readonly state = httpResource<MatchupMap>(() => this.fetchUrl());

    public constructor(private readonly formBuilder: FormBuilder) {
        const initialNames = `Alfa
Bravo
Charlie
Delta
Echo
Foxtrot
Golf`;
        const digitRegex = /^\d+$/;
        this.form = this.formBuilder.nonNullable.group({
            names: [initialNames, Validators.required],
            minGames: ['4', [Validators.required, Validators.pattern(digitRegex)]],
            courtCount: ['1', [Validators.required, Validators.pattern(digitRegex)]],
            shuffle: [false],
        })
    }

    public getControl<T extends keyof Form>(key: T): Form[T] {
        return this.form.controls[key];
    }

    public onSubmit() {
        this.fetchUrl.set(this.getUrl());
    }

    private getUrl() {
        const names = this.getControl('names').value.split('\n').map(n => n.trim());
        if (this.getControl('shuffle').value) {
            shuffle(names);
        }
        const params = new HttpParams({
            fromObject: {
                names,
                minGames: this.getControl('minGames').value,
                courtCount: this.getControl('courtCount').value
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
    names: FormControl<string>;
    minGames: FormControl<string>;
    courtCount: FormControl<string>;
    shuffle: FormControl<boolean>;
}