import { Component, ElementRef, QueryList, ViewChildren, inject, signal, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import {
    MatDialogActions,
    MatDialogContent,
    MatDialogRef,
    MatDialogTitle
} from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';

@Component({
    selector: 'dialog-confirm-mail',
    templateUrl: './dialog-confirm-mail.html',
    styleUrls: ['./dialog-confirm-mail.scss'],
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        MatFormFieldModule,
        MatInputModule,
        MatButtonModule,
        MatDialogTitle,
        MatDialogContent,
        MatDialogActions,
        TranslateModule
    ],
})
export class EmailConfirmDialog implements AfterViewInit {
    readonly dialogRef = inject(MatDialogRef<EmailConfirmDialog>);

    email: string = '';
    expTime: Date | null = null;

    // Signal reattivo per i 6 campi OTP
    readonly otpDigits = signal<string[]>(['', '', '', '', '', '']);

    @ViewChildren('otpInput') otpInputs!: QueryList<ElementRef<HTMLInputElement>>;

    ngAfterViewInit(): void {
        // Focus automatico sul primo campo
        setTimeout(() => this.otpInputs.first?.nativeElement.focus(), 0);
    }

    /** Quando l’utente digita */
    onOtpInput(event: Event, index: number): void {
        const input = event.target as HTMLInputElement;
        let value = input.value.trim();

        // Accetta solo numeri singoli
        if (!/^\d$/.test(value)) {
            input.value = '';
            return;
        }

        // Aggiorna solo la cella modificata
        this.otpDigits.update(digits => {
            const copy = [...digits];
            copy[index] = value;
            return copy;
        });

        // Passa al campo successivo
        const next = input.parentElement?.querySelectorAll('input')[index + 1] as HTMLInputElement;
        next?.focus();
    }

    /** Quando l’utente preme tasti speciali */
    onOtpKeyDown(event: KeyboardEvent, index: number): void {
        const input = event.target as HTMLInputElement;

        // Backspace
        if (event.key === 'Backspace') {
            event.preventDefault();

            this.otpDigits.update(digits => {
                const copy = [...digits];
                copy[index] = '';
                return copy;
            });

            const prev = input.parentElement?.querySelectorAll('input')[index - 1] as HTMLInputElement;
            prev?.focus();
            return;
        }

        // Frecce sinistra/destra
        if (event.key === 'ArrowLeft') {
            const prev = input.parentElement?.querySelectorAll('input')[index - 1] as HTMLInputElement;
            prev?.focus();
        } else if (event.key === 'ArrowRight') {
            const next = input.parentElement?.querySelectorAll('input')[index + 1] as HTMLInputElement;
            next?.focus();
        }
    }

    /** Quando l’utente incolla un codice */
    onPaste(event: ClipboardEvent): void {
        event.preventDefault();

        const input = event.target as HTMLInputElement;

        const data = event.clipboardData?.getData('text') ?? '';
        const digits = data.replace(/\D/g, '').slice(0, 6).split('');

        this.otpDigits.set([...digits, ...Array(6 - digits.length).fill('')]);


        const lastFilledIndex = digits.length > 0 ? digits.length - 1 : 0;
        const focusIndex =
            digits.length < this.otpDigits().length
                ? lastFilledIndex + 1
                : lastFilledIndex;

        const next = input.parentElement?.querySelectorAll('input')[focusIndex] as HTMLInputElement;
        next?.focus();
    }

    onOtpFocus(event: FocusEvent): void {
        event.preventDefault();

        const input = event.target as HTMLInputElement;
        // Se c’è già un valore, lo seleziona tutto
        input.select();
    }


    /** Conferma OTP */
    confirmVerification(): void {
        const code = this.otpDigits().join('');
        console.log('Codice OTP:', code);
        this.dialogRef.close(code);
    }
}
