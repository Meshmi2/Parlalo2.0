using Android.App;
using Android.OS;
using Android.Widget;
using Android.Speech.Tts;
using Android.Content;
using System.Collections.Generic;
using System.Linq;

namespace parlalo6._0
{
    [Activity(MainLauncher = true)]
    public class MainActivity : Activity, TextToSpeech.IOnInitListener
    {
        TextToSpeech textToSpeech;
        Context context;
        private readonly int miocodice = 101, NeedLang = 103;
        Java.Util.Locale lang;



        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_main);
            var btnSayIt = FindViewById<Button>(Resource.Id.btnSpeak);
            var editWhatToSay = FindViewById<EditText>(Resource.Id.editSpeech);
            var spinLanguages = FindViewById<Spinner>(Resource.Id.spinLanguage);
            var txtSpeedVal = FindViewById<TextView>(Resource.Id.textSpeed);
            var txtPitchVal = FindViewById<TextView>(Resource.Id.textPitch);
            var seekSpeed = FindViewById<SeekBar>(Resource.Id.seekSpeed);
            var seekPitch = FindViewById<SeekBar>(Resource.Id.seekPitch);
            seekSpeed.Progress = seekPitch.Progress = 127;
            txtSpeedVal.Text = txtPitchVal.Text = "0.5";
            context = btnSayIt.Context;
            textToSpeech = new TextToSpeech(this, this, "com.google.android.tts");
            var langAvailable = new List<string> { "Default" };
            var localesAvailable = Java.Util.Locale.GetAvailableLocales().ToList();
            foreach (var locale in localesAvailable)
            {
                LanguageAvailableResult res = textToSpeech.IsLanguageAvailable(locale);
                switch (res)
                {
                    case LanguageAvailableResult.Available:
                        langAvailable.Add(locale.DisplayLanguage);
                        break;
                    case LanguageAvailableResult.CountryAvailable:
                         langAvailable.Add(locale.DisplayLanguage);
                        break;
                    case LanguageAvailableResult.CountryVarAvailable:
                         langAvailable.Add(locale.DisplayLanguage);
                        break;
                        }

            }

            langAvailable = langAvailable.OrderBy(t => t).Distinct().ToList();
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, langAvailable);
            spinLanguages.Adapter = adapter;
            lang = Java.Util.Locale.Default;
            textToSpeech.SetLanguage(lang);
            textToSpeech.SetPitch(0.5f);
            textToSpeech.SetSpeechRate(0.5f);
            btnSayIt.Click += delegate
            {
                if (!string.IsNullOrEmpty(editWhatToSay.Text))
                    textToSpeech.Speak(editWhatToSay.Text, QueueMode.Flush, null);
            };
            seekPitch.StopTrackingTouch += (object sender, SeekBar.StopTrackingTouchEventArgs e) =>
            {
                var seek = sender as SeekBar;
                var progress = seek.Progress / 255f;
                textToSpeech.SetPitch(progress);
                txtPitchVal.Text = progress.ToString("F2");
            };
            seekSpeed.StopTrackingTouch += (object sender, SeekBar.StopTrackingTouchEventArgs e) =>
            {
                var seek = sender as SeekBar;
                var progress = seek.Progress / 255f;
                textToSpeech.SetSpeechRate(progress);
                txtSpeedVal.Text = progress.ToString("F2");
            };
            spinLanguages.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {
                lang = Java.Util.Locale.GetAvailableLocales().FirstOrDefault(t => t.DisplayLanguage == langAvailable[(int)e.Id]);
                var checkTTSIntent = new Intent();
                checkTTSIntent.SetAction(TextToSpeech.Engine.ActionCheckTtsData);
                StartActivityForResult(checkTTSIntent, NeedLang);
            };
        }
        void TextToSpeech.IOnInitListener.OnInit(OperationResult status)
        {
            if (status == OperationResult.Error)
                textToSpeech.SetLanguage(Java.Util.Locale.Default);
            if (status == OperationResult.Success)
                textToSpeech.SetLanguage(lang);
                   }
        protected override void OnActivityResult(int req, Result res, Intent data)
        {
            if (req == NeedLang)
            {
                var installTTS = new Intent();
                installTTS.SetAction(TextToSpeech.Engine.ActionInstallTtsData);
                StartActivity(installTTS);
                            }
                    }
            }
}