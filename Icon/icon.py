
from PIL import Image, ImageDraw
import math

icon_size = (256, 256)

icon = Image.new('RGBA', icon_size, (255, 255, 255, 0))
draw = ImageDraw.Draw(icon)

draw.rectangle(((16., 32.), (239., 224.)), fill='white')

wave_colors = [(255, 16, 16, 255), (16, 255, 16, 255), (16, 16, 255, 255)]
wave_amplitudes = [64, 16, 32]
wave_frequencies = [1.0, 1.5, 2.0]
wave_width = 4

for i in range(len(wave_colors)):
    wave_color = wave_colors[i]
    wave_amplitude = wave_amplitudes[i]
    wave_frequency = wave_frequencies[i]
    previous_point = None

    for x in range(17, 239):
        y = int(icon_size[1] / 2 + wave_amplitude * math.sin(2 * math.pi * wave_frequency * x / 196))

        if previous_point is not None:
            draw.line([previous_point, (x, y)], fill=wave_color, width=wave_width)

        previous_point = (x, y)

spectrum_color = (0, 0, 0, 255)
spectrum_line = [(x + 16, int(icon_size[1] / 2)) for x in range(223)]
draw.line(spectrum_line, fill=spectrum_color, width=wave_width)

frame_color = (196, 196, 196, 255)
frame_thickness = 4
draw.rectangle(((16., 32.), (239., 224.)), outline=frame_color, width=frame_thickness)

icon.save('Icon.png')
