import edge_tts
import asyncio
import os

VOICE = "en-IN-NeerjaNeural"

async def generate_audio(text, filename):
    """Generate audio using Edge TTS"""
    communicate = edge_tts.Communicate(text, VOICE, rate="-40%")
    await communicate.save(filename)

# Match your exact Unity folder names
data = [
    ("The man is washing the car", "words", "The man is washing the car."),
    # ("match pushchair with pushchair", "match_words", "match_pushchair_with_pushchair"), 
    # ("show me pushchair", "show_me", "show_me_pushchair"),
]

# Modified path since the script is already inside the Assets folder
BASE_DIR = os.path.join("Resources", "Updated-Audios")

for text, folder, filename_base in data:
    target_folder = os.path.join(BASE_DIR, folder)
    
    # This safely targets your existing subfolders
    os.makedirs(target_folder, exist_ok=True)
    
    filename = os.path.join(target_folder, f"{filename_base}.mp3")
    
    if os.path.exists(filename):
        print(f"Skipped (already exists): {filename}")
        continue
    
    asyncio.run(generate_audio(text, filename))
    print(f"Generated: {filename}")