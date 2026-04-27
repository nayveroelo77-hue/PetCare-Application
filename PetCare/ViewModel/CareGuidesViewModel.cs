using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetCare.Model;
using System.Collections.ObjectModel;

namespace PetCare.ViewModel
{
    public partial class CareGuidesViewModel : BaseViewModel
    {
        public ObservableCollection<CareGuide> Guides { get; set; } = new();

        public CareGuidesViewModel()
        {
            Title = "Pet Care Guides";
            LoadGuides();
        }

        private void LoadGuides()
        {
            Guides.Add(new CareGuide
            {
                Title = "How to Trim Dog/Cat Nails",
                Category = "Grooming",
                ShortDescription = "Learn the safe and stress-free way to trim your pet's nails at home without causing pain.",
                FullContent = "1. Use proper pet nail clippers or grinders.\n" +
                              "2. Identify the 'quick' (the pink part of the nail where nerves and blood vessels are). For dark nails, trim very small slivers at a time.\n" +
                              "3. Hold your pet securely but gently.\n" +
                              "4. Trim the tip off at a 45-degree angle.\n" +
                              "5. If you accidentally cut the quick, use styptic powder to stop the bleeding immediately.\n\n" +
                              "Reward your pet with treats afterwards to build positive associations!",
                Icon = "animal_care.png"
            });

            Guides.Add(new CareGuide
            {
                Title = "Trimming Your Pet's Hair",
                Category = "Grooming",
                ShortDescription = "Basic guidelines for maintaining your pet's coat and trimming overgrown hair.",
                FullContent = "1. Always brush out all tangles and mats BEFORE cutting or washing.\n" +
                              "2. Use round-tipped grooming shears for safety, especially around the face and paws.\n" +
                              "3. Only trim small amounts at a time. Do not cut too close to the skin.\n" +
                              "4. For fully shaving or complex trims (like a Lion cut), consult a professional groomer.\n" +
                              "5. Keep the sessions short and positive, using plenty of treats.",
                Icon = "paw.png"
            });

            Guides.Add(new CareGuide
            {
                Title = "Bathing Frequency Guide",
                Category = "Grooming",
                ShortDescription = "How often should you bathe your pet? Find out the best practices.",
                FullContent = "Dogs generally only need a bath once a month, unless they get incredibly dirty or have a specific skin condition. Over-bathing strips essential oils from their coat, leading to dry and itchy skin.\n\n" +
                              "Cats are meticulous groomers and rarely need baths unless they are unable to groom themselves or have gotten into something toxic/sticky.\n\n" +
                              "Always use pet-specific shampoo, as human shampoo is too harsh for their skin pH.",
                Icon = "animal_care.png"
            });

            Guides.Add(new CareGuide
            {
                Title = "Foods Toxic to Pets",
                Category = "Nutrition",
                ShortDescription = "A quick reference index of common human foods that are highly toxic to cats and dogs.",
                FullContent = "Never feed your pet the following items:\n" +
                              "- Chocolate (especially dark chocolate or cocoa powder)\n" +
                              "- Grapes and Raisins (can cause kidney failure in dogs)\n" +
                              "- Onions and Garlic\n" +
                              "- Xylitol (artificial sweetener found in gum and peanut butter)\n" +
                              "- Macadamia nuts\n" +
                              "- Caffeine and Alcohol\n\n" +
                              "If your pet consumes any of these, contact your emergency vet immediately.",
                Icon = "paw.png"
            });

            Guides.Add(new CareGuide
            {
                Title = "Basic First Aid & CPR",
                Category = "Health",
                ShortDescription = "Essential first aid knowledge every pet owner should know in case of an emergency.",
                FullContent = "Always keep a pet first aid kit handy. It should include gauze, non-stick bandages, adhesive tape, hydrogen peroxide (for inducing vomiting ONLY if directed by a vet), and tweezers.\n\n" +
                              "In case of a wound, apply direct pressure using gauze. Do NOT remove the gauze if it bleeds through, stack more on top.\n\n" +
                              "For CPR: Lay the animal on their right side. Compress the chest 1-2 inches (depending on size) at a rate of 100-120 compressions per minute.",
                Icon = "animal_care.png"
            });
        }

        [RelayCommand]
        private async Task ViewGuideAsync(CareGuide guide)
        {
            if (guide == null) return;
            
            await Shell.Current.DisplayAlert(guide.Title, guide.FullContent, "Back to Guides");
        }
    }
}
