shader_type canvas_item;

uniform vec4 PrimaryKey : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform vec4 AccentKey : source_color = vec4(0.4, 0.4, 0.4, 0.4);
uniform vec4 PrimaryValue : source_color = vec4(0.4, 0.4, 0.4, 0.4);
uniform vec4 AccentValue : source_color = vec4(0.4, 0.4, 0.4, 0.4);

void fragment() {
	if (distance(COLOR.rgb, PrimaryKey.rgb) < 0.1) { COLOR = PrimaryValue; }
	else if (distance(COLOR.rgb, AccentKey.rgb) < 0.1) { COLOR = AccentValue; }
	else if (COLOR.a > 0.9) { COLOR = PrimaryValue; }
}
